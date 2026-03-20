# WorkHub — AI Agent Instructions

## Project Overview
WorkHub là nền tảng quản lý công việc và tri thức cho đội ngũ phát triển phần mềm.
Gồm 5 tính năng chính: Jira Productivity, Task Dashboard, Knowledge Tree, Personal Workspace, Organization Management.

---

## Architecture: Modular Monolith + Clean Architecture

```
WorkHub.sln
├── src/
│   ├── WorkHub.API/              ← Entry point (Controllers, Middlewares, Program.cs)
│   ├── WorkHub.Shared/           ← Shared Kernel (base classes, interfaces, Result<T>)
│   └── Modules/
│       ├── Knowledge/            ← MODULE MẪU — đọc module này trước khi làm module khác
│       │   ├── *.Domain/
│       │   ├── *.Application/
│       │   └── *.Infrastructure/
│       ├── Organization/
│       ├── Tasks/
│       ├── Jira/
│       ├── Workspace/
│       └── AI/
└── tests/
    ├── WorkHub.UnitTests/
    └── WorkHub.IntegrationTests/
```

**Dependency direction (1 chiều, không được đảo ngược):**
```
API → Infrastructure → Application → Domain → Shared
                                         ↑
                              Không import gì cả
```

---

## Absolute Rules — NEVER Violate

1. **Domain KHÔNG import NuGet packages** — Domain chỉ dùng .NET base class library và WorkHub.Shared
2. **Handler KHÔNG gọi thẳng DbContext** — luôn đi qua Repository interface được inject
3. **Không throw raw Exception trong Handler** — dùng `Result<T>.Failure("message")`
4. **Mỗi use case = 1 folder riêng** trong `Commands/` hoặc `Queries/`
5. **Entity constructor phải `private`** — tạo object qua `static` factory method
6. **DbContext phải `internal sealed`** — không expose ra ngoài Infrastructure
7. **Handler phải `internal sealed`** — không expose ra ngoài Application
8. **Mỗi module DI registration trong `DependencyInjection.cs`** tại root của Infrastructure

---

## Tech Stack

| Concern | Library | Version |
|---|---|---|
| Runtime | .NET | 10 |
| CQRS | MediatR | 12.x |
| Validation | FluentValidation | 11.x |
| Mapping | Mapster | 7.x |
| ORM | EF Core + Npgsql | 10.x |
| Search | Elastic.Clients.Elasticsearch | latest |
| Auth | Microsoft.AspNetCore.Authentication.JwtBearer | 10.x |
| Logging | Serilog.AspNetCore | latest |
| AI | Azure.AI.OpenAI | latest |
| Cache | StackExchange.Redis | latest |

---

## Checklist khi thêm 1 tính năng mới

```
Domain layer:
[ ] 1. Tạo/cập nhật Entity (private constructor, static factory method)
[ ] 2. Cập nhật Repository interface nếu cần query mới

Application layer:
[ ] 3. Tạo folder Commands/[ActionName]/ hoặc Queries/[QueryName]/
[ ] 4. Tạo [Name]Command.cs hoặc [Name]Query.cs (record)
[ ] 5. Tạo [Name]Validator.cs (AbstractValidator<T>)
[ ] 6. Tạo [Name]Handler.cs (internal sealed, inject qua interface)

Infrastructure layer:
[ ] 7. Implement method mới trong Repository
[ ] 8. Update EF Core configuration nếu có thay đổi schema

API layer:
[ ] 9. Thêm endpoint vào Controller (chỉ dùng ISender, không inject service trực tiếp)

Tests:
[ ] 10. Viết unit test cho Handler (mock Repository interface)
```

---

## Naming Conventions

```
Command:    [Action][Entity]Command         →  CreateEntryCommand, UpdateFixCommand
Query:      [Get|Search|List][Entity]Query  →  SearchKnowledgeQuery, GetKnowledgeTreeQuery
Handler:    [CommandOrQueryName]Handler     →  CreateEntryCommandHandler
Validator:  [CommandName]Validator          →  CreateEntryCommandValidator
Result DTO: [QueryName]Result               →  SearchKnowledgeResult
Repository: I[Module]Repository             →  IKnowledgeRepository
DbContext:  [Module]DbContext               →  KnowledgeDbContext
DI file:    DependencyInjection.cs          →  luôn đặt ở Infrastructure root
```

---

## Result Pattern — Cách dùng đúng

```csharp
// Handler trả về:
return Result<Guid>.Success(entry.Id);
return Result<Guid>.Failure("Software name is required.");

// Controller kiểm tra:
var result = await _sender.Send(command, ct);
return result.IsSuccess
    ? CreatedAtAction(...)
    : BadRequest(new { error = result.Error });
```

---

## Domain Event Pattern

Khi module A cần thông báo cho module B:
```csharp
// KHÔNG import module B trực tiếp
// Thay vào đó: publish DomainEvent qua MediatR IPublisher
await _publisher.Publish(new TaskCompletedEvent(task.Id), ct);

// Module B đăng ký handler:
public class TaskCompletedEventHandler : INotificationHandler<TaskCompletedEvent> { }
```

---

## Database Conventions

- Mỗi module có **schema riêng**: `knowledge.*`, `tasks.*`, `org.*`, `jira.*`, `workspace.*`
- Connection string key: `"DefaultConnection"` trong appsettings.json
- Migration: mỗi module tự quản lý migrations của mình trong `Infrastructure/Persistence/Migrations/`
- Table naming: `snake_case` (ví dụ: `knowledge_entries`, `task_items`)
- Column naming: `snake_case`

---

## Module mẫu đầy đủ nhất

`src/Modules/Knowledge/` là module được implement đầy đủ nhất.

Trước khi implement module mới, hãy đọc:
1. `WorkHub.Modules.Knowledge.Domain/Entities/KnowledgeEntry.cs` — Entity pattern
2. `WorkHub.Modules.Knowledge.Application/Commands/CreateEntry/` — Command pattern
3. `WorkHub.Modules.Knowledge.Infrastructure/DependencyInjection.cs` — DI pattern

---

## Cấu trúc Controller chuẩn

```csharp
[ApiController]
[Route("api/[module-name]")]
[Authorize]
public class [Module]Controller : ControllerBase
{
    private readonly ISender _sender;  // CHỈ inject ISender, không inject service khác

    public [Module]Controller(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateXxxRequest request, CancellationToken ct)
    {
        var result = await _sender.Send(new CreateXxxCommand(..., GetCurrentUserId()), ct);
        return result.IsSuccess ? CreatedAtAction(...) : BadRequest(new { error = result.Error });
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst("sub") ?? User.FindFirst("userId");
        return claim is not null && Guid.TryParse(claim.Value, out var id)
            ? id
            : throw new UnauthorizedAccessException("User identity not found.");
    }
}
```
