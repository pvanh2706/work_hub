# Jira Frontend API Integration - Design Specification

**Date:** 2026-03-22  
**Author:** AI + User collaboration  
**Status:** Approved  
**Related:** Backend JiraController.cs

---

## Overview

### Goal
Integrate Jira backend API endpoints vào frontend Vue 3 application, creating a service layer foundation that can be reused for other modules (Knowledge, Tasks, AI, Organization, Workspace).

### Scope
**In scope:**
- Types/interfaces matching backend DTOs
- API service layer với Result<T> pattern
- Vue 3 composables với reactive state
- Hybrid error handling (auto notification với silent option)

**Out of scope (Phase 2):**
- UI components (modals, forms, pages)
- Pinia store integration
- Advanced features (caching, optimistic updates, offline support)

### Backend API Endpoints

5 endpoints từ `JiraController.cs`:

1. `POST /api/jira/issues` - Create issue
2. `PUT /api/jira/issues/{issueKey}` - Edit issue
3. `GET /api/jira/issues/{issueKey}` - Get issue details
4. `GET /api/jira/issues/{issueKey}/transitions` - Get available transitions
5. `POST /api/jira/issues/{issueKey}/transitions` - Transition issue status

---

## Architecture Decisions

### AD-1: Feature-based Organization
**Decision:** Group Jira code trong `services/jira/` folder thay vì scatter across `types/`, `services/`, `composables/`

**Rationale:**
- All Jira-related code ở một chỗ → easier navigation
- Types, API, composables are tightly coupled → keep together
- Scales better (có thể thêm `jira/utils.ts`, `jira/constants.ts` sau)
- Consistent với backend module structure

**Alternative considered:** Domain-driven (types/ separate từ services/) - rejected vì over-engineering cho current scale

### AD-2: Result<T> Pattern for API Responses
**Decision:** Return `{ isSuccess: boolean, data?: T, error?: string }` từ API layer

**Rationale:**
- Matches backend C# Result<T> pattern → consistent mental model
- Type-safe error handling
- Explicit success/failure states
- No throwing exceptions in normal flow

**Alternative considered:** Try-catch with thrown errors - rejected vì less predictable, harder to handle in Vue

### AD-3: Hybrid Error Handling in Composables
**Decision:** Composables auto show ElNotification for errors, với `silent: true` option để override

**Rationale:**
- Default behavior: user-friendly (errors visible immediately)
- Flexible: components có thể custom error handling khi cần
- DRY: không repeat notification logic trong mọi component
- Progressive enhancement: start simple, optimize later

**Alternative considered:** 
- Always silent (component handles) - rejected vì too much boilerplate
- Always show notification - rejected vì không flexible

### AD-4: Composable Factory Pattern
**Decision:** Dùng `createComposable<TRequest, TResponse>()` factory

**Rationale:**
- DRY: loading/error logic chỉ viết 1 lần
- Consistent API: all composables có same interface
- Easy to extend: add retry, caching, etc. ở một chỗ
- Type-safe: TypeScript generics preserve types

---

## File Structure

```
FrontEnd/src/
├── services/
│   ├── api/
│   │   └── axios.ts              # Existing - Axios instance
│   ├── jira/                     # ⭐ NEW
│   │   ├── index.ts              # Export all public APIs
│   │   ├── types.ts              # Enums, interfaces, DTOs
│   │   ├── api.ts                # HTTP calls returning Result<T>
│   │   └── composables.ts        # Vue reactive wrappers
│   └── user.ts                   # Existing
└── types/
    └── common.ts                 # NEW - Shared Result<T> type
```

### File Responsibilities

**types.ts:**
- TypeScript enums (IssueType, IssuePriority)
- Request DTOs (CreateIssueRequest, EditIssueRequest, TransitionIssueRequest)
- Response DTOs (JiraIssue, JiraTransition)
- NO logic, pure data structures

**api.ts:**
- Generic `apiCall<T>()` wrapper for axios
- Object export: `jiraApi.createIssue()`, `jiraApi.editIssue()`, etc.
- Returns `ApiResponse<T>` (Result pattern)
- No Vue dependencies

**composables.ts:**
- `createComposable<TRequest, TResponse>()` factory
- Specific composables: `useCreateIssue()`, `useEditIssue()`, etc.
- Manages: `loading` ref, `result` ref, `execute()` function
- Hybrid error handling với ElNotification

**index.ts:**
- Re-exports everything: `export * from './types'`, etc.
- Single import point: `import { useCreateIssue, IssueType } from '@/services/jira'`

---

## Type Definitions

### Enums

```typescript
// Numeric values match C# backend
export enum IssueType {
  Bug = 0,
  Feature = 1,
  Task = 2,
  Story = 3,
  Epic = 4,
  Subtask = 5
}

export enum IssuePriority {
  Lowest = 0,
  Low = 1,
  Medium = 2,
  High = 3,
  Highest = 4,
  Critical = 5
}
```

**Rationale for numeric:** Backend serializes enums as numbers, direct mapping without conversion.

### Request DTOs

```typescript
export interface CreateIssueRequest {
  organizationId: string        // C# Guid → JSON string
  projectKey: string
  summary: string
  description: string
  issueType: IssueType
  priority: IssuePriority
  issueTypeId: string          // Jira internal ID
  priorityId: string           // Jira internal ID
  assigneeAccountId?: string   // Optional
  labels?: string[]            // Optional
}

export interface EditIssueRequest {
  summary?: string
  description?: string
  priorityId?: string
  assigneeAccountId?: string
  labelsToAdd?: string[]
  labelsToRemove?: string[]
}

export interface TransitionIssueRequest {
  transitionId: string
  comment?: string
}
```

**Mapping rules:**
- C# `Guid` → TypeScript `string`
- C# nullable (`string?`) → TypeScript optional (`string?`)
- C# `List<T>` → TypeScript `T[]`

### Response DTOs

```typescript
export interface JiraIssue {
  key: string                  // e.g., "PROJ-123"
  summary: string
  description: string
  status: string
  issueType: string
  priority: string
  assignee?: string
  created: string              // ISO 8601 datetime
  updated: string
  url: string
}

export interface JiraTransition {
  id: string
  name: string
  to: {
    id: string
    name: string
  }
}
```

### Result Type

```typescript
// types/common.ts - Reusable across all services
export interface ApiResponse<T> {
  isSuccess: boolean
  data?: T
  error?: string
}
```

---

## API Service Layer

### Generic Wrapper

```typescript
import api from '@/services/api/axios'
import type { AxiosError } from 'axios'

async function apiCall<T>(
  method: 'get' | 'post' | 'put' | 'delete',
  endpoint: string,
  data?: any
): Promise<ApiResponse<T>> {
  try {
    // GET/DELETE use params, POST/PUT use body
    const response = method === 'get' || method === 'delete'
      ? await api[method]<T>(endpoint, data ? { params: data } : undefined)
      : await api[method]<T>(endpoint, data)
    
    return { isSuccess: true, data: response.data }
  } catch (error) {
    const axiosError = error as AxiosError<{ error?: string }>
    return {
      isSuccess: false,
      error: axiosError.response?.data?.error || axiosError.message || 'Unknown error'
    }
  }
}
```

**Features:**
- Generic type parameter for response
- Consistent error extraction
- Try-catch contained here (không leak ra caller)
- Returns Result pattern

### Jira API Functions

```typescript
import type { 
  CreateIssueRequest, 
  EditIssueRequest, 
  TransitionIssueRequest,
  JiraIssue,
  JiraTransition,
  ApiResponse
} from './types'

/**
 * Jira API service object
 * All functions return ApiResponse<T> with Result pattern
 */
export const jiraApi = {
  /**
   * Creates a new Jira issue in the specified project
   * @param request - Issue creation data including summary, description, type, priority
   * @returns API response with created issue key and URL
   * @example
   * const result = await jiraApi.createIssue({
   *   organizationId: '123e4567-e89b-12d3-a456-426614174000',
   *   projectKey: 'PROJ',
   *   summary: 'Fix login bug',
   *   description: 'Users cannot login',
   *   issueType: IssueType.Bug,
   *   priority: IssuePriority.High,
   *   issueTypeId: '10001',
   *   priorityId: '3'
   * })
   */
  async createIssue(request: CreateIssueRequest): Promise<ApiResponse<{ issueKey: string; url: string }>> {
    return apiCall('post', '/api/jira/issues', request)
  },

  async editIssue(issueKey: string, request: EditIssueRequest): Promise<ApiResponse<void>> {
    return apiCall('put', `/api/jira/issues/${issueKey}`, request)
 mport { ref, type Ref } from 'vue'
import { ElNotification } from 'element-plus'
import { jiraApi } from './api'
import type { ApiResponse } from './types'

i },

  async getIssue(issueKey: string): Promise<ApiResponse<JiraIssue>> {
    return apiCall('get', `/api/jira/issues/${issueKey}`)
  },

  async getTransitions(issueKey: string): Promise<ApiResponse<JiraTransition[]>> {
    return apiCall('get', `/api/jira/issues/${issueKey}/transitions`)
  },

  async transitionIssue(issueKey: string, request: TransitionIssueRequest): Promise<ApiResponse<void>> {
    return apiCall('post', `/api/jira/issues/${issueKey}/transitions`, request)
  }
}
```

**Design principles:**
- 1:1 mapping với backend endpoints
- Object export (`jiraApi.*`) for namespacing
- JSDoc comments for each function
- Pure functions - no side effects

---

## Composables Layer

### Factory Pattern

```typescript
interface ComposableOptions {
  silent?: boolean  // Default: false (show notifications)
}

function createComposable<TRequest, TResponse>(
  apiCall: (request: TRequest) => Promise<ApiResponse<TResponse>>
) {
  return (options: ComposableOptions = {}) => {
    const loading = ref(false)
    const result: Ref<ApiResponse<TResponse> | null> = ref(null)
    
    const execute = async (request: TRequest) => {
      loading.value = true
      
      try {
        const response = await apiCall(request)
        result.value = response
        
        // Hybrid error handling
        if (!response.isSuccess && !options.silent) {
          ElNotification({
            title: 'Lỗi',
            message: response.error || 'Có lỗi xảy ra',
            type: 'error',
            duration: 3000
          })
        }
        
        return response
      } finally {
        loading.value = false
      }
    }
    
    return { loading, result, execute }
  }
}
```

**Benefits:**
- DRY: Common logic in one place
- Consistent: All composables have same structure
- Extensible: Easy to add success notifications, retry, etc.
- Type-safe: Generics preserve request/response types

### Specific Composables

```typescript
// Simple case - direct mapping
export const useCreateIssue = createComposable<
  CreateIssueRequest,
  { issueKey: string; url: string }
>(jiraApi.createIssue)

// Complex case - multiple params need adapter
export const useEditIssue = () => {
  const { loading, result, execute: baseExecute } = createComposable<
    { issueKey: string; request: EditIssueRequest },
    void
  >(({ issueKey, request }) => jiraApi.editIssue(issueKey, request))()
  
  const execute = (issueKey: string, request: EditIssueRequest) => {
    return baseExecute({ issueKey, request })
  }
  
  return { loading, result, execute }
}
```

**Pattern explained:**
- Simple composables: direct export của factory
- Complex composables (multiple params): adapter layer để clean API

### Usage inRouter } from 'vue-router'
import { useCreateIssue, IssueType, IssuePriority } from '@/services/jira'

const router = useRouter()
```typescript
// Example 1: Default behavior (show error notification)
<script setup lang="ts">
import { useCreateIssue, IssueType, IssuePriority } from '@/services/jira'

const { loading, result, execute } = useCreateIssue()

const createIssue = async () => {
  const formData = {
    organizationId: '...',
    projectKey: 'PROJ',
    summary: 'Bug title',
    description: 'Description',
    issueType: IssueType.Bug,
    priority: IssuePriority.High,
    issueTypeId: '10001',
    priorityId: '3'
  }
  
  await execute(formData)
  
  if (result.value?.isSuccess) {
    console.log('Created:', result.value.data.issueKey)
    router.push(`/jira/${result.value.data.issueKey}`)
  }
  // Error đã tự động show notification
}
</script>

<template>
  <button @click="createIssue" :disabled="loading">
    <span v-if="loading">Creating...</span>
    <span v-else>Create Issue</span>
  </button>
</template>
```

```typescript
// Example 2: Custom error handling (silent mode)
const { execute } = useCreateIssue({ silent: true })

const createIssue = async () => {
  const result = await execute(formData)
  
  if (!result.isSuccess) {
    // Custom error UI
    showMyErrorDialog({
      title: 'Failed to create issue',
      message: result.error,
      actions: ['Retry', 'Cancel']
    })
  }
}
```

---

## Implementation Phases

### Phase 1: Foundation (This Design)
**Files:** 5 files
1. `types/common.ts` - ApiResponse<T>
2. `services/jira/types.ts` - Enums + DTOs
3. `services/jira/api.ts` - API calls
4. `services/jira/composables.ts` - Vue wrappers
5. `services/jira/index.ts` - Exports

**Testing:** Manual testing với backend running

**Deliverable:** Service layer usable from components

### Phase 2: UI Components (Future)
- Create issue modal
- Issue detail view
- Transition dropdown
- Form với validation (Zod?)

### Phase 3: Advanced Features (Future)
- Pinia store cho shared state
- Caching strategy
- Optimistic updates
- Offline queue
- Retry logic

---

## Success Criteria

### Must Have
- ✅ All 5 backend endpoints có corresponding composables
- ✅ Types match backend DTOs exactly
- ✅ Result<T> pattern implemented correctly
- ✅ Error notifications show by default
- ✅ Silent option works
- ✅ TypeScript strict mode passes
- ✅ No Vue warnings in console

### Nice to Have
- 🎯 JSDoc comments complete
- 🎯 Usage examples in comments
- 🎯 Consistent naming conventions
- 🎯 Clean index.ts exports

### Out of Scope
- UI components
- Validation logic (done in components)
- Caching
- Tests (will add later)

---

## Open Questions

None - all design decisions approved by user.

---

## References

- Backend: `d:\MiniProject\WorkHub\BackEnd\WorkHub\src\WorkHub.API\Controllers\JiraController.cs`
- Frontend base: `d:\MiniProject\WorkHub\FrontEnd\src\services\api\axios.ts`
- Existing pattern: `d:\MiniProject\WorkHub\FrontEnd\src\services\user.ts`

---

**Next Steps:** Writing-plans skill to create implementation plan
