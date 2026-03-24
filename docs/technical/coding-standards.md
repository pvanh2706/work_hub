# Coding Standards

Quy chuẩn code cho WorkHub project.

## General Principles

- **SOLID principles**
- **DRY** (Don't Repeat Yourself)
- **YAGNI** (You Aren't Gonna Need It)
- **KISS** (Keep It Simple, Stupid)

## C# Backend Standards

### Naming Conventions

```csharp
// Classes, Interfaces, Methods: PascalCase
public class KnowledgeEntry { }
public interface IKnowledgeRepository { }
public void CreateEntry() { }

// Private fields: _camelCase
private readonly IMediator _mediator;

// Parameters, local variables: camelCase
public void Method(string parameterName)
{
    var localVariable = "";
}

// Constants: PascalCase
public const string DefaultValue = "value";
```

### Entity Pattern

```csharp
public class KnowledgeEntry : AuditableEntity
{
    // Properties - private set
    public string Title { get; private set; } = default!;
    
    // Collections - private backing field
    private readonly List<string> _tags = [];
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();
    
    // Constructor MUST be private
    private KnowledgeEntry() { }
    
    // Factory method
    public static KnowledgeEntry Create(...)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        return new KnowledgeEntry { Title = title, ... };
    }
    
    // Domain methods
    public void UpdateTitle(string newTitle)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newTitle);
        Title = newTitle;
        SetUpdated(updatedBy);
    }
}
```

### CQRS Pattern

**Command:**
```csharp
public record CreateEntryCommand(
    string Title,
    string Content,
    Guid CreatedBy
) : ICommand<Result<Guid>>;
```

**Validator:**
```csharp
public class CreateEntryCommandValidator : AbstractValidator<CreateEntryCommand>
{
    public CreateEntryCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty();
    }
}
```

**Handler:**
```csharp
internal sealed class CreateEntryCommandHandler 
    : ICommandHandler<CreateEntryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateEntryCommand cmd, 
        CancellationToken ct)
    {
        // Validation
        if (validationFails)
            return Result<Guid>.Failure("Error message");
        
        // Business logic
        var entry = KnowledgeEntry.Create(...);
        await _repository.AddAsync(entry, ct);
        
        return Result<Guid>.Success(entry.Id);
    }
}
```

### Result Pattern

```csharp
// ✅ DO: Return Result<T>
public async Task<Result<Guid>> Handle(...)
{
    if (node is null)
        return Result<Guid>.Failure("Node not found");
    
    return Result<Guid>.Success(entityId);
}

// ❌ DON'T: Throw exceptions for business failures
public async Task<Guid> Handle(...)
{
    if (node is null)
        throw new NotFoundException("Node", id); // Bad!
    
    return entityId;
}
```

### File Organization

```
Module/
├── Domain/
│   ├── Entities/          # One entity per file
│   ├── Enums/
│   ├── Events/
│   └── Repositories/      # Interfaces only
├── Application/
│   ├── Abstractions/      # Interfaces
│   ├── Commands/
│   │   └── CreateEntry/
│   │       ├── CreateEntryCommand.cs
│   │       ├── CreateEntryCommandValidator.cs
│   │       └── CreateEntryCommandHandler.cs
│   └── Queries/
│       └── SearchEntries/
│           ├── SearchEntriesQuery.cs
│           ├── SearchEntriesQueryHandler.cs
│           └── SearchEntriesResult.cs
└── Infrastructure/
    ├── DependencyInjection.cs
    ├── Persistence/
    │   ├── {Module}DbContext.cs
    │   ├── Configurations/
    │   └── Migrations/
    └── Repositories/
```

## TypeScript Frontend Standards

### Naming Conventions

```typescript
// Interfaces/Types: PascalCase
interface User { }
type UserRole = 'admin' | 'user';

// Variables, functions: camelCase
const userName = 'John';
function getUserName() { }

// Constants: UPPER_SNAKE_CASE
const API_BASE_URL = 'https://api.example.com';

// Components: PascalCase
const UserProfile = defineComponent({ });
```

### Vue 3 Composition API

```vue
<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'

// Props
interface Props {
  title: string
  count?: number
}

const props = withDefaults(defineProps<Props>(), {
  count: 0
})

// Emits
interface Emits {
  (e: 'update', value: number): void
  (e: 'close'): void
}

const emit = defineEmits<Emits>()

// Reactive state
const isLoading = ref(false)
const items = ref<Item[]>([])

// Computed
const itemCount = computed(() => items.value.length)

// Methods
const handleClick = () => {
  emit('update', props.count + 1)
}

// Lifecycle
onMounted(() => {
  loadData()
})
</script>

<template>
  <div class="container">
    <h1>{{ props.title }}</h1>
    <p>Count: {{ itemCount }}</p>
  </div>
</template>

<style scoped>
.container {
  padding: 1rem;
}
</style>
```

### API Service Pattern

```typescript
// services/api.ts
import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export default api

// services/knowledge.ts
export interface CreateEntryRequest {
  title: string
  content: string
}

export const knowledgeService = {
  async createEntry(data: CreateEntryRequest) {
    return api.post('/knowledge/entries', data)
  },
  
  async getEntry(id: string) {
    return api.get(`/knowledge/entries/${id}`)
  }
}
```

## Testing Standards

### Unit Tests

```csharp
// Arrange-Act-Assert pattern
public class CreateEntryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateEntryCommand(...);
        var handler = new CreateEntryCommandHandler(...);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
    }
    
    [Fact]
    public async Task Handle_InvalidTitle_ReturnsFailure()
    {
        // Arrange
        var command = new CreateEntryCommand("", ...); // Empty title
        var handler = new CreateEntryCommandHandler(...);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("title", result.Error.ToLower());
    }
}
```

### Integration Tests

```csharp
public class KnowledgeApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    [Fact]
    public async Task POST_CreateEntry_ReturnsCreated()
    {
        // Arrange
        var request = new { title = "Test", content = "Content" };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/knowledge/entries", request);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
```

## Git Commit Messages

Format: `<type>(<scope>): <subject>`

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `refactor`: Code refactoring
- `test`: Adding tests
- `chore`: Maintenance

**Examples:**
```
feat(jira): add quick issue creation command
fix(knowledge): resolve search indexing bug
docs(api): update authentication endpoints
refactor(tasks): simplify task state machine
test(knowledge): add entry validation tests
chore(deps): update Entity Framework to 10.0.5
```

## Code Review Checklist

- [ ] Follows naming conventions
- [ ] No duplicated code
- [ ] Tests added/updated
- [ ] Documentation updated
- [ ] No hardcoded values
- [ ] Error handling present
- [ ] Logging added where appropriate
- [ ] Performance considered

## References

- [Clean Architecture](../ARCHITECTURE.md)
- [CQRS Patterns](cqrs-patterns.md)
- [Testing Guide](testing.md)

---

**Last Updated:** 2026-03-22
