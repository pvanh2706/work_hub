# Jira Frontend API Integration - Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Integrate Jira backend API vào Vue 3 frontend với service layer foundation sử dụng Result<T> pattern và composables

**Architecture:** Feature-based organization (`services/jira/`) với 3 layers: Types → API Service (Result pattern) → Composables (Vue reactive). Hybrid error handling với ElNotification default.

**Tech Stack:** TypeScript, Vue 3 Composition API, Axios, Element Plus

**Related Spec:** [docs/superpowers/specs/2026-03-22-jira-frontend-api-integration.md](../specs/2026-03-22-jira-frontend-api-integration.md)

---

## File Structure Overview

**New files to create (5 files):**
```
FrontEnd/src/
├── types/
│   └── common.ts                         # ApiResponse<T> type
└── services/
    └── jira/
        ├── types.ts                      # Enums, DTOs
        ├── api.ts                        # HTTP calls with Result pattern
        ├── composables.ts                # Vue reactive wrappers
        └── index.ts                      # Public exports
```

**Existing files to reference (not modify):**
- `FrontEnd/src/services/api/axios.ts` - Axios instance (already configured)
- `BackEnd/WorkHub/src/WorkHub.API/Controllers/JiraController.cs` - Backend reference

---

## Task 1: Create Common Types

**Files:**
- Create: `FrontEnd/src/types/common.ts`

### Step 1.1: Create ApiResponse<T> type

- [ ] **Create common.ts with Result pattern type**

```typescript
/**
 * Generic API response wrapper using Result pattern
 * Matches backend Result<T> structure
 */
export interface ApiResponse<T> {
  /** Indicates if the API call succeeded */
  isSuccess: boolean
  
  /** Response data (only present when isSuccess = true) */
  data?: T
  
  /** Error message (only present when isSuccess = false) */
  error?: string
}

/**
 * Type guard to narrow ApiResponse to success case
 * @example
 * if (isSuccess(response)) {
 *   console.log(response.data) // TypeScript knows data exists
 * }
 */
export function isSuccess<T>(
  response: ApiResponse<T>
): response is { isSuccess: true; data: T } {
  return response.isSuccess
}
```

- [ ] **Verify TypeScript compilation**

Run in terminal:
```bash
cd FrontEnd
npm run type-check
```

Expected: No errors

- [ ] **Commit common types**

```bash
git add src/types/common.ts
git commit -m "feat(types): add ApiResponse<T> with Result pattern

- Generic response wrapper for all API calls
- Type guard for narrowing success cases
- Matches backend Result<T> structure"
```

---

## Task 2: Create Jira Types & DTOs

**Files:**
- Create: `FrontEnd/src/services/jira/types.ts`

### Step 2.1: Create enums

- [ ] **Create jira/types.ts with enums**

```typescript
/**
 * Jira issue type enumeration
 * Values match C# backend enum exactly (numeric)
 */
export enum IssueType {
  Bug = 0,
  Feature = 1,
  Task = 2,
  Story = 3,
  Epic = 4,
  Subtask = 5
}

/**
 * Jira issue priority enumeration
 * Values match C# backend enum exactly (numeric)
 */
export enum IssuePriority {
  Lowest = 0,
  Low = 1,
  Medium = 2,
  High = 3,
  Highest = 4,
  Critical = 5
}
```

### Step 2.2: Add request DTOs

- [ ] **Add CreateIssueRequest interface**

Append to `types.ts`:

```typescript
/**
 * Request DTO for creating a new Jira issue
 * Matches backend CreateIssueRequest exactly
 */
export interface CreateIssueRequest {
  /** Organization ID (Guid as string in JSON) */
  organizationId: string
  
  /** Jira project key (e.g., "PROJ") */
  projectKey: string
  
  /** Issue title/summary */
  summary: string
  
  /** Issue description (can be markdown) */
  description: string
  
  /** Issue type enum value */
  issueType: IssueType
  
  /** Priority enum value */
  priority: IssuePriority
  
  /** Jira internal issue type ID */
  issueTypeId: string
  
  /** Jira internal priority ID */
  priorityId: string
  
  /** Assignee account ID (optional) */
  assigneeAccountId?: string
  
  /** Labels array (optional) */
  labels?: string[]
}

/**
 * Request DTO for editing existing Jira issue
 * All fields optional - only send what changes
 */
export interface EditIssueRequest {
  summary?: string
  description?: string
  priorityId?: string
  assigneeAccountId?: string
  labelsToAdd?: string[]
  labelsToRemove?: string[]
}

/**
 * Request DTO for transitioning issue status
 */
export interface TransitionIssueRequest {
  /** Jira transition ID */
  transitionId: string
  
  /** Optional comment to add during transition */
  comment?: string
}
```

### Step 2.3: Add response DTOs

- [ ] **Add response interfaces**

Append to `types.ts`:

```typescript
/**
 * Jira issue details response
 * Returned from GET /api/jira/issues/{key}
 */
export interface JiraIssue {
  /** Issue key (e.g., "PROJ-123") */
  key: string
  
  /** Issue summary/title */
  summary: string
  
  /** Issue description */
  description: string
  
  /** Current status name */
  status: string
  
  /** Issue type name */
  issueType: string
  
  /** Priority name */
  priority: string
  
  /** Assignee display name (optional) */
  assignee?: string
  
  /** Creation timestamp (ISO 8601) */
  created: string
  
  /** Last updated timestamp (ISO 8601) */
  updated: string
  
  /** Direct URL to issue in Jira */
  url: string
}

/**
 * Jira transition option
 * Returned from GET /api/jira/issues/{key}/transitions
 */
export interface JiraTransition {
  /** Transition ID */
  id: string
  
  /** Transition name (e.g., "In Progress", "Done") */
  name: string
  
  /** Destination status details */
  to: {
    id: string
    name: string
  }
}
```

### Step 2.4: Re-export common type

- [ ] **Add import for ApiResponse**

Add at top of `types.ts`:

```typescript
export type { ApiResponse } from '@/types/common'
```

- [ ] **Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: No errors, all types resolved

- [ ] **Commit Jira types**

```bash
git add src/services/jira/types.ts
git commit -m "feat(jira): add TypeScript types and DTOs

- Enums: IssueType, IssuePriority (match backend)
- Request DTOs: Create, Edit, Transition
- Response DTOs: JiraIssue, JiraTransition
- Re-export ApiResponse from common types"
```

---

## Task 3: Create API Service Layer

**Files:**
- Create: `FrontEnd/src/services/jira/api.ts`

### Step 3.1: Create generic API wrapper

- [ ] **Create api.ts with apiCall helper**

```typescript
import api from '@/services/api/axios'
import type { AxiosError } from 'axios'
import type { ApiResponse } from './types'

/**
 * Generic API call wrapper with Result pattern
 * Handles GET/DELETE vs POST/PUT method differences
 * 
 * @param method - HTTP method
 * @param endpoint - API endpoint path
 * @param data - Request data (params for GET/DELETE, body for POST/PUT)
 * @returns ApiResponse with Result pattern
 */
async function apiCall<T>(
  method: 'get' | 'post' | 'put' | 'delete',
  endpoint: string,
  data?: any
): Promise<ApiResponse<T>> {
  try {
    // GET/DELETE use query params, POST/PUT use request body
    const response = method === 'get' || method === 'delete'
      ? await api[method]<T>(endpoint, data ? { params: data } : undefined)
      : await api[method]<T>(endpoint, data)
    
    return {
      isSuccess: true,
      data: response.data
    }
  } catch (error) {
    const axiosError = error as AxiosError<{ error?: string }>
    
    return {
      isSuccess: false,
      error: axiosError.response?.data?.error || axiosError.message || 'Unknown error'
    }
  }
}
```

### Step 3.2: Add Jira API functions

- [ ] **Add jiraApi object with createIssue**

Append to `api.ts`:

```typescript
import type { 
  CreateIssueRequest, 
  EditIssueRequest, 
  TransitionIssueRequest,
  JiraIssue,
  JiraTransition
} from './types'

/**
 * Jira API service
 * All functions return ApiResponse<T> following Result pattern
 */
export const jiraApi = {
  /**
   * Creates a new Jira issue in the specified project
   * @param request - Issue creation data
   * @returns API response with created issue key and URL
   * @example
   * const result = await jiraApi.createIssue({
   *   organizationId: '123e4567-e89b-12d3-a456-426614174000',
   *   projectKey: 'PROJ',
   *   summary: 'Fix login bug',
   *   description: 'Users cannot login on mobile',
   *   issueType: IssueType.Bug,
   *   priority: IssuePriority.High,
   *   issueTypeId: '10001',
   *   priorityId: '3'
   * })
   */
  async createIssue(
    request: CreateIssueRequest
  ): Promise<ApiResponse<{ issueKey: string; url: string }>> {
    return apiCall('post', '/api/jira/issues', request)
  },

  /**
   * Updates an existing Jira issue
   * @param issueKey - Issue key (e.g., "PROJ-123")
   * @param request - Fields to update
   */
  async editIssue(
    issueKey: string,
    request: EditIssueRequest
  ): Promise<ApiResponse<void>> {
    return apiCall('put', `/api/jira/issues/${issueKey}`, request)
  },

  /**
   * Retrieves Jira issue details
   * @param issueKey - Issue key (e.g., "PROJ-123")
   */
  async getIssue(
    issueKey: string
  ): Promise<ApiResponse<JiraIssue>> {
    return apiCall('get', `/api/jira/issues/${issueKey}`)
  },

  /**
   * Gets available transitions for an issue
   * @param issueKey - Issue key (e.g., "PROJ-123")
   */
  async getTransitions(
    issueKey: string
  ): Promise<ApiResponse<JiraTransition[]>> {
    return apiCall('get', `/api/jira/issues/${issueKey}/transitions`)
  },

  /**
   * Transitions issue to a new status
   * @param issueKey - Issue key (e.g., "PROJ-123")
   * @param request - Transition details
   */
  async transitionIssue(
    issueKey: string,
    request: TransitionIssueRequest
  ): Promise<ApiResponse<void>> {
    return apiCall('post', `/api/jira/issues/${issueKey}/transitions`, request)
  }
}
```

- [ ] **Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: No errors, all imports resolved

- [ ] **Commit API service**

```bash
git add src/services/jira/api.ts
git commit -m "feat(jira): add API service layer with Result pattern

- Generic apiCall<T> wrapper handling GET/POST/PUT/DELETE
- 5 API functions mapping to backend endpoints:
  - createIssue, editIssue, getIssue
  - getTransitions, transitionIssue
- JSDoc comments with examples
- Type-safe with full DTO typing"
```

---

## Task 4: Create Composables Layer

**Files:**
- Create: `FrontEnd/src/services/jira/composables.ts`

### Step 4.1: Create composable factory

- [ ] **Create composables.ts with factory pattern**

```typescript
import { ref, type Ref } from 'vue'
import { ElNotification } from 'element-plus'
import { jiraApi } from './api'
import type { ApiResponse } from './types'

/**
 * Options for composable behavior
 */
interface ComposableOptions {
  /** If true, do not show error notifications (default: false) */
  silent?: boolean
}

/**
 * Generic composable factory
 * Creates Vue composables that wrap API calls with reactive state
 * 
 * @param apiCall - API function to wrap
 * @returns Composable function that returns { loading, result, execute }
 */
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
        
        // Hybrid error handling: show notification unless silent
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
    
    return {
      loading,
      result,
      execute
    }
  }
}
```

### Step 4.2: Add specific composables

- [ ] **Add useCreateIssue composable**

Append to `composables.ts`:

```typescript
import type { 
  CreateIssueRequest,
  EditIssueRequest,
  TransitionIssueRequest,
  JiraIssue,
  JiraTransition
} from './types'

/**
 * Composable for creating Jira issues
 * 
 * @param options - Composable options (silent mode, etc.)
 * @returns { loading, result, execute }
 * 
 * @example
 * const { loading, result, execute } = useCreateIssue()
 * await execute(createIssueData)
 * if (result.value?.isSuccess) {
 *   console.log('Created:', result.value.data.issueKey)
 * }
 */
export const useCreateIssue = createComposable<
  CreateIssueRequest,
  { issueKey: string; url: string }
>(jiraApi.createIssue)
```

- [ ] **Add useEditIssue composable**

Append to `composables.ts`:

```typescript
/**
 * Composable for editing Jira issues
 * Provides cleaner API by accepting issueKey as separate parameter
 * 
 * @example
 * const { loading, execute } = useEditIssue()
 * await execute('PROJ-123', { summary: 'Updated title' })
 */
export function useEditIssue(options: ComposableOptions = {}) {
  const { loading, result, execute: baseExecute } = createComposable<
    { issueKey: string; request: EditIssueRequest },
    void
  >(({ issueKey, request }) => jiraApi.editIssue(issueKey, request))(options)
  
  const execute = (issueKey: string, request: EditIssueRequest) => {
    return baseExecute({ issueKey, request })
  }
  
  return { loading, result, execute }
}
```

- [ ] **Add useGetIssue composable**

Append to `composables.ts`:

```typescript
/**
 * Composable for fetching Jira issue details
 * 
 * @example
 * const { loading, result, execute } = useGetIssue()
 * await execute('PROJ-123')
 * if (result.value?.isSuccess) {
 *   console.log(result.value.data.summary)
 * }
 */
export function useGetIssue(options: ComposableOptions = {}) {
  const { loading, result, execute: baseExecute } = createComposable<
    string,
    JiraIssue
  >(jiraApi.getIssue)(options)
  
  const execute = (issueKey: string) => {
    return baseExecute(issueKey)
  }
  
  return { loading, result, execute }
}
```

- [ ] **Add useGetTransitions composable**

Append to `composables.ts`:

```typescript
/**
 * Composable for fetching available issue transitions
 * 
 * @example
 * const { loading, result, execute } = useGetTransitions()
 * await execute('PROJ-123')
 * if (result.value?.isSuccess) {
 *   const transitions = result.value.data
 *   transitions.forEach(t => console.log(t.name))
 * }
 */
export function useGetTransitions(options: ComposableOptions = {}) {
  const { loading, result, execute: baseExecute } = createComposable<
    string,
    JiraTransition[]
  >(jiraApi.getTransitions)(options)
  
  const execute = (issueKey: string) => {
    return baseExecute(issueKey)
  }
  
  return { loading, result, execute }
}
```

- [ ] **Add useTransitionIssue composable**

Append to `composables.ts`:

```typescript
/**
 * Composable for transitioning issue status
 * 
 * @example
 * const { loading, execute } = useTransitionIssue()
 * await execute('PROJ-123', {
 *   transitionId: '31',
 *   comment: 'Moving to Done'
 * })
 */
export function useTransitionIssue(options: ComposableOptions = {}) {
  const { loading, result, execute: baseExecute } = createComposable<
    { issueKey: string; request: TransitionIssueRequest },
    void
  >(({ issueKey, request }) => jiraApi.transitionIssue(issueKey, request))(options)
  
  const execute = (issueKey: string, request: TransitionIssueRequest) => {
    return baseExecute({ issueKey, request })
  }
  
  return { loading, result, execute }
}
```

- [ ] **Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: No errors

- [ ] **Commit composables**

```bash
git add src/services/jira/composables.ts
git commit -m "feat(jira): add Vue composables with reactive state

- createComposable<T> factory for DRY composable creation
- Hybrid error handling (ElNotification with silent option)
- 5 composables: useCreateIssue, useEditIssue, useGetIssue,
  useGetTransitions, useTransitionIssue
- Reactive loading and result state
- Type-safe with full generics support"
```

---

## Task 5: Create Public API Exports

**Files:**
- Create: `FrontEnd/src/services/jira/index.ts`

### Step 5.1: Create barrel export file

- [ ] **Create index.ts with all exports**

```typescript
/**
 * Jira service public API
 * 
 * Import everything from here:
 * @example
 * import { useCreateIssue, IssueType, jiraApi } from '@/services/jira'
 */

// Types & Enums
export * from './types'

// API Service
export * from './api'

// Composables
export * from './composables'
```

- [ ] **Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: All exports valid, no circular dependencies

- [ ] **Verify imports work from components**

Create temporary test file to verify:

```bash
# In FrontEnd directory
cat > src/test-jira-imports.ts << 'EOF'
// Test file - delete after verification
import { 
  useCreateIssue, 
  IssueType, 
  IssuePriority,
  jiraApi,
  type CreateIssueRequest,
  type JiraIssue 
} from '@/services/jira'

console.log('IssueType.Bug:', IssueType.Bug)
console.log('IssuePriority.High:', IssuePriority.High)

const { loading, execute } = useCreateIssue()
console.log('useCreateIssue loaded:', typeof execute)
console.log('jiraApi loaded:', typeof jiraApi.createIssue)
EOF

npm run type-check
```

Expected: No errors, all types resolved

- [ ] **Delete test file**

```bash
rm src/test-jira-imports.ts
```

- [ ] **Commit index exports**

```bash
git add src/services/jira/index.ts
git commit -m "feat(jira): add barrel export for public API

- Single import point: @/services/jira
- Exports types, enums, api, composables
- Clean public API surface"
```

---

## Task 6: Integration Verification

**Files:**
- None (testing only)

### Step 6.1: Verify TypeScript strict mode

- [ ] **Run type-check with strict mode**

Ensure `tsconfig.json` has strict mode enabled:

```bash
# Check tsconfig
cat tsconfig.json | grep -A5 '"strict"'

# Run type-check
npm run type-check
```

Expected output:
```
✓ Type-check completed without errors
```

### Step 6.2: Verify no Vue warnings

- [ ] **Start dev server and check console**

```bash
# Terminal 1: Start frontend
npm run dev

# Open browser: http://localhost:5173
# Open DevTools Console
```

Expected: No warnings about missing dependencies, types, or module resolution

- [ ] **Stop dev server**

```bash
# Ctrl+C in terminal
```

### Step 6.3: Manual API test (if backend running)

- [ ] **Optional: Test actual API call**

If backend is running at `https://localhost:7001`, create a test component:

```vue
<!-- src/views/TestJira.vue -->
<script setup lang="ts">
import { useCreateIssue, IssueType, IssuePriority } from '@/services/jira'

const { loading, result, execute } = useCreateIssue()

const testCreate = async () => {
  await execute({
    organizationId: 'test-org-id',
    projectKey: 'PROJ',
    summary: 'Test issue from frontend',
    description: 'Testing integration',
    issueType: IssueType.Bug,
    priority: IssuePriority.Medium,
    issueTypeId: '10001',
    priorityId: '2'
  })
  
  console.log('Result:', result.value)
}
</script>

<template>
  <div>
    <button @click="testCreate" :disabled="loading">
      {{ loading ? 'Creating...' : 'Test Create Issue' }}
    </button>
    <pre v-if="result">{{ JSON.stringify(result, null, 2) }}</pre>
  </div>
</template>
```

Add route to test component, navigate to it, click button.

Expected: Either issue created (if org/project exist) or error message (if not) - both indicate API layer working.

**Delete test component after verification.**

---

## Task 7: Final Documentation

**Files:**
- Update: `FrontEnd/README.md` (optional)

### Step 7.1: Document usage in README

- [ ] **Add Jira service section to README**

If `FrontEnd/README.md` exists, add section:

```markdown
## Jira Service Integration

Service layer cho Jira API integration.

**Location:** `src/services/jira/`

**Usage:**
```typescript
import { useCreateIssue, IssueType, IssuePriority } from '@/services/jira'

const { loading, result, execute } = useCreateIssue()

await execute({
  organizationId: '...',
  projectKey: 'PROJ',
  summary: 'Issue title',
  description: 'Description',
  issueType: IssueType.Bug,
  priority: IssuePriority.High,
  issueTypeId: '10001',
  priorityId: '3'
})

if (result.value?.isSuccess) {
  console.log('Created:', result.value.data.issueKey)
}
```

**Silent mode (custom error handling):**
```typescript
const { execute } = useCreateIssue({ silent: true })
const result = await execute(data)
if (!result.isSuccess) {
  showCustomErrorDialog(result.error)
}
```

**API Reference:** See [spec](../docs/superpowers/specs/2026-03-22-jira-frontend-api-integration.md)
```

- [ ] **Commit documentation**

```bash
git add FrontEnd/README.md
git commit -m "docs: add Jira service usage guide to README"
```

---

## Completion Checklist

Verify all tasks completed:

- [ ] Task 1: Common types created (ApiResponse<T>)
- [ ] Task 2: Jira types created (enums, DTOs)
- [ ] Task 3: API service created (jiraApi object)
- [ ] Task 4: Composables created (5 composables)
- [ ] Task 5: Index exports created
- [ ] Task 6: Integration verified
- [ ] Task 7: Documentation updated

**Final verification:**
```bash
# Check all files exist
ls src/types/common.ts
ls src/services/jira/*.ts

# Type-check passes
npm run type-check

# No uncommitted changes
git status
```

**Expected file count:** 5 new files in git

---

## Post-Implementation Notes

### What We Built
- ✅ Service layer foundation for Jira API
- ✅ Result<T> pattern matching backend
- ✅ Vue 3 composables with reactive state
- ✅ Hybrid error handling (auto + manual)
- ✅ Full TypeScript strict mode support

### What's Next (Phase 2)
- UI components (modals, forms, issue detail view)
- Pinia store for shared Jira state
- Form validation với Zod
- Caching strategy
- Optimistic updates
- Retry logic

### Reusable Patterns
The patterns created here can be reused for:
- Knowledge Module API integration
- Tasks Module API integration
- AI Module API integration
- Organization Module API integration
- Workspace Module API integration

Just copy `services/jira/` structure và adapt DTOs/endpoints.

---

**Plan Status:** Ready for implementation  
**Estimated Time:** 1-2 hours (all 7 tasks)  
**Skill to use:** `superpowers:subagent-driven-development` (recommended)
