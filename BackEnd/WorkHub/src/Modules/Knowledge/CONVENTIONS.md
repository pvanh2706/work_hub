# Knowledge Module — Conventions & Patterns

Module này là **module mẫu** của toàn hệ thống WorkHub.
Khi implement module mới, follow y chang cấu trúc này.

---

## Cấu trúc thư mục

```
WorkHub.Modules.Knowledge.Domain/
├── Entities/
│   ├── KnowledgeNode.cs          ← Node trong cây (Software → Module → Issue)
│   └── KnowledgeEntry.cs         ← Nội dung tri thức (rootCause + fix)
├── Enums/
│   └── KnowledgeNodeType.cs      ← Software = 1, Module = 2, Issue = 3
├── Events/
│   └── KnowledgeEntryCreatedEvent.cs  ← Domain event cho module khác subscribe
└── Repositories/
    └── IKnowledgeRepository.cs   ← Interface — Infrastructure sẽ implement

WorkHub.Modules.Knowledge.Application/
├── Abstractions/
│   ├── ISearchIndexer.cs         ← Interface cho search (Elasticsearch, Meilisearch...)
│   └── IKnowledgeSearchService.cs
├── Commands/
│   ├── CreateEntry/
│   │   ├── CreateEntryCommand.cs          ← record — dữ liệu đầu vào
│   │   ├── CreateEntryCommandValidator.cs ← FluentValidation rules
│   │   └── CreateEntryCommandHandler.cs   ← internal sealed — business logic
│   └── UpdateFix/
│       └── ...
└── Queries/
    ├── SearchKnowledge/
    │   ├── SearchKnowledgeQuery.cs
    │   ├── SearchKnowledgeQueryHandler.cs
    │   └── SearchKnowledgeResult.cs       ← DTO trả về cho client
    └── GetKnowledgeTree/
        └── ...

WorkHub.Modules.Knowledge.Infrastructure/
├── DependencyInjection.cs         ← Entry point — đăng ký tất cả services
├── Persistence/
│   ├── KnowledgeDbContext.cs      ← internal sealed
│   ├── Configurations/
│   │   └── KnowledgeConfigurations.cs  ← IEntityTypeConfiguration
│   └── Migrations/
├── Repositories/
│   └── KnowledgeRepository.cs    ← Implement IKnowledgeRepository
└── Search/
    └── ElasticsearchServices.cs  ← Implement ISearchIndexer + IKnowledgeSearchService
```

---

## Entity Pattern

```csharp
public class KnowledgeEntry : AuditableEntity   // Kế thừa AuditableEntity từ Shared
{
    // Properties — private set
    public string IssueTitle { get; private set; } = default!;

    // Collections — private backing field
    private readonly List<string> _tags = [];
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    // Constructor PHẢI private (EF Core dùng reflection, vẫn hoạt động)
    private KnowledgeEntry() { }

    // Factory method — enforce business rules
    public static KnowledgeEntry Create(
        Guid nodeId, string issueTitle, ...)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(issueTitle);  // Guard clauses
        return new KnowledgeEntry { ... };
    }

    // Domain methods — business operations
    public void UpdateFix(string newFix, Guid updatedBy)
    {
        Fix = newFix;
        SetUpdated(updatedBy);  // Method từ AuditableEntity
    }
}
```

---

## Command + Validator + Handler Pattern

### Command (record — immutable)
```csharp
// Commands/CreateEntry/CreateEntryCommand.cs
public record CreateEntryCommand(
    string SoftwareName,
    string IssueTitle,
    string RootCause,
    string Fix,
    Guid CreatedBy          // ← Lấy từ JWT, không để client truyền lên
) : IRequest<Result<Guid>>;
```

### Validator
```csharp
// Commands/CreateEntry/CreateEntryCommandValidator.cs
public class CreateEntryCommandValidator : AbstractValidator<CreateEntryCommand>
{
    public CreateEntryCommandValidator()
    {
        RuleFor(x => x.SoftwareName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IssueTitle).NotEmpty().MaximumLength(500);
        // JiraIssueKey chỉ validate khi có giá trị
        RuleFor(x => x.JiraIssueKey)
            .Matches(@"^[A-Z]+-\d+$")
            .When(x => !string.IsNullOrEmpty(x.JiraIssueKey));
    }
}
```

### Handler
```csharp
// Commands/CreateEntry/CreateEntryCommandHandler.cs
internal sealed class CreateEntryCommandHandler    // internal sealed — không expose
    : IRequestHandler<CreateEntryCommand, Result<Guid>>
{
    // Inject interfaces, KHÔNG inject concrete classes
    private readonly IKnowledgeRepository _repository;
    private readonly ISearchIndexer _searchIndexer;

    public async Task<Result<Guid>> Handle(CreateEntryCommand request, CancellationToken ct)
    {
        // 1. Business logic
        // 2. Gọi Repository (không gọi DbContext trực tiếp)
        // 3. Trả về Result<T>
        return Result<Guid>.Success(entry.Id);
    }
}
```

---

## Repository Interface Pattern

```csharp
// Domain/Repositories/IKnowledgeRepository.cs
public interface IKnowledgeRepository : IRepository<KnowledgeEntry>  // Kế thừa IRepository<T> từ Shared
{
    // Chỉ khai báo method cần thiết cho business logic
    // KHÔNG expose IQueryable — tránh leak query logic lên Application layer
    Task<KnowledgeNode> FindOrCreateNodeAsync(string software, string module, Guid createdBy, CancellationToken ct);
    Task AddEntryAsync(KnowledgeEntry entry, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
```

---

## DependencyInjection Pattern

```csharp
// Infrastructure/DependencyInjection.cs
public static class DependencyInjection
{
    // Extension method — được gọi từ Program.cs
    public static IServiceCollection AddKnowledgeModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. DbContext
        services.AddDbContext<KnowledgeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // 2. Repositories
        services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();

        // 3. Infrastructure services (implement Application interfaces)
        services.AddScoped<ISearchIndexer, ElasticsearchIndexer>();
        services.AddScoped<IKnowledgeSearchService, ElasticsearchSearchService>();

        return services;
    }
}
```

---

## Quy tắc đặc thù của Knowledge module

1. **Tree structure**: Node có thể lồng nhau tối đa 3 cấp (Software → Module → Issue node)
2. **Tags**: Luôn lowercase và trim trước khi lưu
3. **JiraIssueKey**: Luôn uppercase (ví dụ: `PMS-1234`), validate format `^[A-Z]+-\d+$`
4. **HelpfulVotes**: Chỉ tăng, không giảm — dùng `MarkHelpful()` method
5. **Search**: Ưu tiên `IssueTitle` (boost 3x) > `RootCause` (boost 2x) > `Fix` > `Tags`
