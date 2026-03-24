# Jira Module - Architecture

Clean Architecture implementation following WorkHub patterns.

## Module Structure

```
WorkHub.Modules.Jira/
├── Domain/                     # Core business logic
│   ├── Entities/
│   │   ├── IssueTemplate.cs
│   │   ├── JiraIssueSync.cs
│   │   └── WorkLogEntry.cs
│   ├── Enums/
│   │   ├── IssueType.cs
│   │   ├── IssuePriority.cs
│   │   └── SyncStatus.cs
│   ├── Events/
│   │   ├── IssueCreatedEvent.cs
│   │   ├── IssueSyncedEvent.cs
│   │   └── TemplateCreatedEvent.cs
│   └── Repositories/
│       ├── IIssueTemplateRepository.cs
│       └── IJiraIssueSyncRepository.cs
├── Application/                # Use cases
│   ├── Commands/
│   │   ├── CreateIssue/
│   │   │   ├── CreateIssueCommand.cs
│   │   │   ├── CreateIssueCommandValidator.cs
│   │   │   └── CreateIssueCommandHandler.cs
│   │   └── SyncFromJira/
│   └── Queries/
│       ├── GetIssues/
│       └── SearchIssues/
├── Infrastructure/             # External concerns
│   ├── Persistence/
│   │   ├── JiraDbContext.cs
│   │   ├── Configurations/
│   │   └── Migrations/
│   ├── Jira/
│   │   ├── JiraApiClient.cs
│   │   └── JiraWebhookHandler.cs
│   └── DependencyInjection.cs
└── Presentation/               # API endpoints
    └── JiraController.cs
```

## Design Patterns

### 1. CQRS (Command Query Responsibility Segregation)

**Commands - Change state:**
```csharp
public record CreateIssueCommand(
    string Title,
    string Description,
    IssueType Type,
    Guid CreatedBy
) : ICommand<Result<Guid>>;

internal sealed class CreateIssueCommandHandler 
    : ICommandHandler<CreateIssueCommand, Result<Guid>>
{
    private readonly IJiraApiClient _jiraApi;
    private readonly IIssueTemplateRepository _templateRepo;
    
    public async Task<Result<Guid>> Handle(
        CreateIssueCommand cmd,
        CancellationToken ct)
    {
        // 1. Get template
        var template = await _templateRepo.GetByType(cmd.Type, ct);
        
        // 2. Apply template
        var issue = JiraIssue.Create(cmd.Title, cmd.Description, template);
        
        // 3. Create in Jira
        var jiraKey = await _jiraApi.CreateIssue(issue, ct);
        if (jiraKey.IsFailure)
            return Result<Guid>.Failure(jiraKey.Error);
        
        // 4. Store locally
        issue.SetJiraKey(jiraKey.Value);
        await _repository.AddAsync(issue, ct);
        
        return Result<Guid>.Success(issue.Id);
    }
}
```

**Queries - Read data:**
```csharp
public record SearchIssuesQuery(
    string SearchTerm,
    IssueType? Type,
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<IssueDto>>;

internal sealed class SearchIssuesQueryHandler 
    : IQueryHandler<SearchIssuesQuery, PagedResult<IssueDto>>
{
    private readonly IJiraDbContext _dbContext;
    
    public async Task<PagedResult<IssueDto>> Handle(
        SearchIssuesQuery query,
        CancellationToken ct)
    {
        var queryable = _dbContext.JiraIssues
            .AsNoTracking()
            .Where(i => i.Title.Contains(query.SearchTerm));
        
        if (query.Type.HasValue)
            queryable = queryable.Where(i => i.Type == query.Type.Value);
        
        var total = await queryable.CountAsync(ct);
        
        var issues = await queryable
            .OrderByDescending(i => i.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ProjectTo<IssueDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
        
        return new PagedResult<IssueDto>(issues, total, query.PageNumber, query.PageSize);
    }
}
```

### 2. Result<T> Pattern

**No exceptions for business failures:**
```csharp
public class Result<T>
{
    public T Value { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    
    public static Result<T> Success(T value) => new(value, true, string.Empty);
    public static Result<T> Failure(string error) => new (default!, false, error);
}

// Usage
public async Task<Result<Guid>> CreateIssue(CreateIssueCommand cmd)
{
    if (string.IsNullOrWhiteSpace(cmd.Title))
        return Result<Guid>.Failure("Title is required");
    
    // ... create logic
    
    return Result<Guid>.Success(issueId);
}

// Calling code
var result = await mediator.Send(new CreateIssueCommand(...));
if (result.IsFailure)
    return BadRequest(result.Error);

return Ok(result.Value);
```

### 3. Factory Pattern

**Entities with private constructors:**
```csharp
public class IssueTemplate : AuditableEntity
{
    public string Name { get; private set; } = default!;
    public IssueType Type { get; private set; }
    public TemplateFields Fields { get; private set; } = default!;
    
    // Private constructor - cannot instantiate directly
    private IssueTemplate() { }
    
    // Factory method - Only way to create valid entity
    public static IssueTemplate Create(
        string name,
        IssueType type,
        TemplateFields fields,
        Guid createdBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(fields);
        
        var template = new IssueTemplate
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type,
            Fields = fields
        };
        
        template.SetCreated(createdBy);
        
        return template;
    }
    
    // Domain methods for state changes
    public void UpdateName(string newName, Guid updatedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);
        
        Name = newName;
        SetUpdated(updatedBy);
    }
}
```

### 4. Repository Pattern

**Abstract data access:**
```csharp
public interface IIssueTemplateRepository
{
    Task<IssueTemplate?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IssueTemplate?> GetByType(IssueType type, CancellationToken ct);
    Task<List<IssueTemplate>> GetAllAsync(CancellationToken ct);
    Task AddAsync(IssueTemplate template, CancellationToken ct);
    Task UpdateAsync(IssueTemplate template, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

// Implementation
internal sealed class IssueTemplateRepository : IIssueTemplateRepository
{
    private readonly JiraDbContext _context;
    
    public async Task<IssueTemplate?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.IssueTemplates
            .Include(t => t.Fields)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    
    public async Task AddAsync(IssueTemplate template, CancellationToken ct)
    {
        await _context.IssueTemplates.AddAsync(template, ct);
        await _context.SaveChangesAsync(ct);
    }
}
```

## Data Flow

```
┌────────────────────────────────────────────────────────────┐
│                        User Request                         │
└────────────────────┬───────────────────────────────────────┘
                     │
                     ▼
┌────────────────────────────────────────────────────────────┐
│                  JiraController (API)                       │
│  • Validate request                                         │
│  • Map to command                                           │
│  • Send to MediatR                                          │
└────────────────────┬───────────────────────────────────────┘
                     │
                     ▼
┌────────────────────────────────────────────────────────────┐
│               CommandHandler (Application)                  │
│  • Validate business rules                                  │
│  • Load domain entities                                     │
│  • Execute domain logic                                     │
│  • Call external services (Jira API)                       │
│  • Persist changes                                          │
│  • Publish domain events                                    │
└────────────────────┬───────────────────────────────────────┘
                     │
        ┌────────────┼────────────┐
        │            │            │
        ▼            ▼            ▼
┌──────────┐  ┌──────────┐  ┌──────────┐
│ Domain   │  │   Jira   │  │ Database │
│ Entities │  │   API    │  │          │
└──────────┘  └──────────┘  └──────────┘
```

## Domain Events

**Publish when important state changes:**
```csharp
public record IssueCreatedEvent(
    Guid IssueId,
    string Title,
    IssueType Type,
    string JiraKey,
    Guid CreatedBy,
    DateTime CreatedAt
) : IDomainEvent;

// Handler
internal sealed class IssueCreatedEventHandler 
    : IDomainEventHandler<IssueCreatedEvent>
{
    private readonly INotificationService _notifications;
    private readonly IKnowledgeService _knowledge;
    
    public async Task Handle(IssueCreatedEvent evt, CancellationToken ct)
    {
        // 1. Notify assignee
        await _notifications.NotifyIssueCreated(evt.IssueId, ct);
        
        // 2. Check for similar issues in knowledge base
        await _knowledge.FindSimilarIssues(evt.Title, ct);
        
        // 3. Log analytics event
        await _analytics.TrackIssueCreated(evt);
    }
}
```

**Event publishers:**
```
IssueCreatedEvent → [IssueCreatedEventHandler, AnalyticsHandler, NotificationHandler]
IssueSyncedEvent → [CacheInvalidator, WebhookNotifier]
TemplateCreatedEvent → [AuditLogger]
```

## Clean Architecture Layers

### Domain Layer (Core)
**Dependencies:** None  
**Contains:**
- Entities (IssueTemplate, JiraIssueSync)
- Value Objects (JiraKey, TemplateVariable)
- Domain Events
- Repository Interfaces
- Enums

**Rules:**
- ❌ No dependencies on outer layers
- ❌ No dependencies on frameworks
- ✅ Pure business logic only

### Application Layer
**Dependencies:** Domain layer only  
**Contains:**
- Commands & Queries (CQRS)
- Validators (FluentValidation)
- Command/Query Handlers
- Application Services
- DTOs

**Rules:**
- ❌ No dependencies on Infrastructure
- ❌ No dependencies on Presentation
- ✅ Orchestrates use cases
- ✅ Uses repository interfaces

### Infrastructure Layer
**Dependencies:** Domain + Application  
**Contains:**
- DbContext & EF Core configs
- Repository implementations
- External API clients (Jira)
- Migrations
- Dependency Injection setup

**Rules:**
- ✅ Can reference Domain & Application
- ✅ Implements interfaces from Domain
- ✅ Contains all framework code

### Presentation Layer (API)
**Dependencies:** Application layer  
**Contains:**
- Controllers
- Middleware
- Request/Response models
- API documentation

**Rules:**
- ✅ Thin layer - delegates to Application
- ✅ Handles HTTP concerns only
- ❌ No business logic

## Dependency Injection

**Module registration:**
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddJiraModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<JiraDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString(\"DefaultConnection\"),
                b => b.MigrationsAssembly(typeof(JiraDbContext).Assembly.FullName)
            ));
        
        // Repositories
        services.AddScoped<IIssueTemplateRepository, IssueTemplateRepository>();
        services.AddScoped<IJiraIssueSyncRepository, JiraIssueSyncRepository>();
        
        // External services
        services.AddHttpClient<IJiraApiClient, JiraApiClient>(client =>
        {
            client.BaseAddress = new Uri(configuration[\"Jira:BaseUrl\"]!);
            client.DefaultRequestHeaders.Add(\"Accept\", \"application/json\");
        });
        
        // MediatR
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
```

## Configuration

**appsettings.json:**
```json
{
  \"Jira\": {
    \"BaseUrl\": \"https://your-domain.atlassian.net\",
    \"Email\": \"your-email@company.com\",
    \"ApiToken\": \"your-api-token\",
    \"DefaultProject\": \"PROJ\",
    \"WebhookSecret\": \"webhook-secret-key\",
    \"RateLimiting\": {
      \"RequestsPerMinute\": 100,
      \"BurstSize\": 20
    },
    \"Sync\": {
      \"PollingIntervalMinutes\": 15,
      \"WebhooksEnabled\": true,
      \"ConflictResolutionStrategy\": \"last-write-wins\"
    }
  }
}
```

## Next Steps

- [API Endpoints](api-endpoints.md) - REST API reference
- [Database Schema](database-schema.md) - Tables and relationships
- [Domain Models](domain-models.md) - Entity details
- [Jira API Integration](jira-api-integration.md) - External service integration

---

**Last Updated:** 2026-03-22
