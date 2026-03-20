# Architecture Decision Records — WorkHub

Tài liệu này giải thích **lý do** đằng sau các quyết định kiến trúc quan trọng.
Mục đích: giúp người mới và AI agent hiểu "tại sao" chứ không chỉ "cái gì".

---

## ADR-001: Modular Monolith thay vì Microservices

**Quyết định:** Dùng Modular Monolith  
**Ngày:** 2026-03-15

### Bối cảnh
Team nhỏ (1-3 người), sản phẩm còn ở giai đoạn MVP, chưa biết module nào sẽ cần scale.

### Lý do KHÔNG dùng Microservices ngay
- Microservices yêu cầu infrastructure phức tạp: service discovery, distributed tracing, API gateway
- Overhead của network calls giữa services làm chậm development ở giai đoạn đầu
- Team nhỏ không đủ người để maintain nhiều services độc lập

### Lý do chọn Modular Monolith
- Deploy như 1 app → infrastructure đơn giản
- Code được tổ chức như microservices → dễ tách sau này
- Mỗi module có Domain/Application/Infrastructure riêng → khi cần tách, copy 3 folder sang repo mới

### Khi nào tách microservices?
Khi 1 trong các điều kiện sau xảy ra:
- Một module cần scale độc lập (ví dụ: AI module bị bottleneck)
- Team đủ lớn để maintain services riêng biệt (>10 người)
- Business logic của 2 module bắt đầu diverge mạnh

---

## ADR-002: Clean Architecture với 3 tầng per module

**Quyết định:** Domain / Application / Infrastructure là 3 project riêng biệt  
**Ngày:** 2026-03-15

### Bối cảnh
Cần một kiến trúc cho phép test nhanh và swap technology.

### Lý do chọn 3 project riêng (không phải 3 folder trong 1 project)

**Compiler enforce rules:**
```
❌ Nếu để chung 1 project: không có gì ngăn dev gọi DbContext từ Entity
✅ Project riêng: compiler báo lỗi ngay nếu Domain import EF Core
```

**Test speed:**
```
Unit test (Application + Domain):  ~milliseconds — không cần DB
Integration test (Infrastructure):  ~seconds — cần Docker/DB
```

**Technology swap:**
```
Đổi Elasticsearch → Meilisearch:
  - Chỉ thay file trong Infrastructure/Search/
  - Application và Domain KHÔNG thay đổi
  - Vì Application chỉ biết ISearchIndexer interface
```

### Dependency Rule
```
API → Infrastructure → Application → Domain → Shared
```
Dependency chỉ đi một chiều. Vi phạm rule này = thiết kế sai.

---

## ADR-003: Result<T> Pattern thay vì Exceptions

**Quyết định:** Handler trả về `Result<T>`, không throw exception  
**Ngày:** 2026-03-15

### Lý do
```csharp
// ❌ Exception-based: caller không biết method có thể fail
public async Task<Guid> CreateEntry(CreateEntryCommand cmd)
{
    throw new NotFoundException("Node", id);  // Caller bất ngờ
}

// ✅ Result-based: failure là một phần của contract
public async Task<Result<Guid>> Handle(CreateEntryCommand cmd)
{
    if (node is null) return Result<Guid>.Failure("Node not found");
    return Result<Guid>.Success(entry.Id);
}
```

**Lợi ích:**
- Caller bắt buộc xử lý cả success và failure
- Dễ test hơn (không cần try-catch trong test)
- Performance tốt hơn (exception stack unwinding tốn CPU)

**Ngoại lệ:** Vẫn dùng exception cho:
- Unexpected system errors (DB connection lost)
- Programming errors (null argument, invalid state)
- Authentication/Authorization violations

---

## ADR-004: MediatR cho CQRS

**Quyết định:** Dùng MediatR, không inject Service class trực tiếp vào Controller  
**Ngày:** 2026-03-15

### Lý do

**Controller chỉ phụ thuộc vào ISender:**
```csharp
// ✅ Đúng — Controller không biết gì về business logic
public class KnowledgeController(ISender sender) : ControllerBase

// ❌ Sai — Controller biết quá nhiều về implementation
public class KnowledgeController(IKnowledgeService service, ISearchService search, ...) : ControllerBase
```

**Cross-cutting concerns qua Pipeline Behaviors:**
```
Request → ValidationBehavior → LoggingBehavior → Handler → Response
```
Thêm behavior mới không cần sửa Handler.

---

## ADR-005: Mỗi module có DbContext riêng

**Quyết định:** Không share DbContext giữa các modules  
**Ngày:** 2026-03-15

### Lý do

**Schema isolation:**
```sql
-- knowledge.* chỉ do Knowledge module quản lý
-- tasks.* chỉ do Tasks module quản lý
-- Không có foreign key cross-schema
```

**Migration isolation:**
```
Mỗi module tự add-migration, update-database
Migrations của module A không ảnh hưởng module B
```

**Lợi ích khi tách microservice:**
```
Tách Knowledge thành service riêng:
  → Chỉ cần copy KnowledgeDbContext và migrations
  → Không bị ràng buộc với DB của module khác
```

### Giao tiếp cross-module
Modules KHÔNG query DB của nhau. Giao tiếp qua:
1. **Domain Events** (MediatR INotification) — async, fire-and-forget style
2. **API calls** — khi cần response ngay (hiếm dùng trong monolith)

---

## ADR-006: Entity Factory Method Pattern

**Quyết định:** Entity constructor `private`, expose `public static Create(...)` method  
**Ngày:** 2026-03-15

### Lý do

```csharp
// ❌ Public constructor: không enforce business rules khi tạo
var entry = new KnowledgeEntry();  // Missing required fields
entry.RootCause = "";              // Bypass validation

// ✅ Factory method: enforce invariants ngay lúc tạo
var entry = KnowledgeEntry.Create(
    nodeId, title, description, rootCause, fix, ...);
// => ArgumentException.ThrowIfNullOrWhiteSpace ngay trong Create()
```

**EF Core vẫn hoạt động:** EF Core dùng reflection để tạo entity khi load từ DB — `private constructor` không ảnh hưởng.

---

## Module Boundaries — Ai làm gì

| Module | Trách nhiệm | KHÔNG làm |
|---|---|---|
| **Organization** | Users, Roles, Permissions, OrganizationId | Không quản lý business logic của module khác |
| **Tasks** | TaskItem, WorkSession, Kanban state | Không sync Jira trực tiếp — fire event |
| **Jira** | Jira API integration, IssueTemplate, Sync | Không lưu task state — đó là của Tasks |
| **Knowledge** | KnowledgeNode tree, KnowledgeEntry, Search | Không tạo Jira issue |
| **Workspace** | SecureVault (encrypted), PersonalNote | Không share vault giữa users |
| **AI** | OpenAI integration, GenerateIssue, SuggestRootCause | Không lưu kết quả — gọi module khác để lưu |
