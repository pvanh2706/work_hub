# Spec: Jira Configuration UI

**Date:** 2026-03-23  
**Status:** Draft  
**Author:** GitHub Copilot  
**Reviewers:** TBD

---

## 1. Overview

### 1.1 Purpose
Create a comprehensive UI for managing Jira connection configuration in WorkHub. This feature allows administrators to connect WorkHub to a Jira instance, test the connection, fetch available projects, and set a default project for the workspace.

### 1.2 Scope
- **Configuration Level:** Global (single Jira instance for entire WorkHub application)
- **Feature Set:** Full featured with connection management, testing, project fetching, and metadata display
- **Access Control:** 
  - View permissions: All users can see connection status
  - Edit permissions: Admin only can modify/delete configuration
- **UI Flow:** Adaptive UI that switches between Empty State and Connected State

### 1.3 Goals
1. Provide intuitive interface for Jira connection setup
2. Enable connection validation before saving
3. Support project discovery and default project selection
4. Display connection metadata for transparency
5. Implement proper permission-based UI rendering
6. Ensure type-safe API integration with ApiResponse<T> pattern

### 1.4 Non-Goals
- Organization-level or workspace-level configuration (Phase 2)
- Multiple Jira instance support (Phase 2)
- Advanced field mapping or custom workflows (Phase 3)
- Jira webhook configuration (Phase 3)

### 1.5 Prerequisites

**Before implementation begins, verify:**

1. **Backend API Endpoints Available:**
   - `GET /api/jira/config` - Get configuration
   - `POST /api/jira/config` - Save configuration
   - `POST /api/jira/config/test` - Test connection
   - `DELETE /api/jira/config` - Delete configuration
   - `GET /api/jira/config/projects` - Fetch projects
   - `PUT /api/jira/config/default-project` - Set default project
   
   **Status:** ⚠️ NOT CONFIRMED - Backend team must verify availability

2. **Permission/Auth System:**
   - User authentication is implemented
   - User roles are defined (Admin, User, Guest)
   - `userStore.hasRole()` method available OR alternative permission check
   
   **Status:** ⚠️ NOT IMPLEMENTED - Phase 1 workaround: Mock admin permissions (always true) for testing

3. **Shared Infrastructure:**
   - ✅ `apiCall()` utility exists at `src/utils/api.ts`
   - ✅ `ApiResponse<T>` type and `isSuccess()` guard at `src/types/common.ts`
   - ✅ Element Plus components available
   - ✅ Vue Router configured

**Action Required Before Proceeding:**
- Confirm backend endpoints exist or create implementation plan
- Decide permission strategy: Implement full auth OR use mock for Phase 1

---

## 2. Architecture

### 2.1 File Structure

```
FrontEnd/src/
├── views/Admin/JiraConfig/
│   ├── JiraConfig.vue                    # Main container/orchestrator
│   └── components/
│       ├── JiraConnectionForm.vue        # Credentials input form
│       ├── JiraConnectionInfo.vue        # Readonly connection display
│       └── JiraProjectSettings.vue       # Projects list + metadata
│
├── components/jira/
│   ├── JiraStatusBadge.vue               # Reusable status indicator
│   └── JiraProjectSelector.vue           # Reusable project dropdown
│
├── composables/jira/
│   ├── useJiraConfigState.ts             # Shared reactive state (singleton)
│   ├── useJiraConnection.ts              # Connection operations
│   └── useJiraProjects.ts                # Project operations
│
├── services/jira/
│   └── config-api.ts                     # Configuration API endpoints
│
└── types/jira/
    └── config.ts                         # Type definitions
```

**Total Files:** 11 (4 view components, 2 reusable components, 3 composables, 1 API service, 1 types file)

**Naming Convention Note:**
- `config-api.ts` is separate from existing `api.ts` (task operations)
- This separation keeps configuration concerns distinct from task/project/issue operations
- Future: `api.ts` handles Jira tasks/issues, `config-api.ts` handles connection settings

---

## 3. Component Architecture

### 3.1 JiraConfig.vue (Main Container)

**Location:** `views/Admin/JiraConfig/JiraConfig.vue`

**Responsibilities:**
- Root component orchestrating all child components
- Fetches initial config on mount
- Handles permission checks (admin vs regular user)
- Manages UI state transitions (Empty ↔ Connected)
- Coordinates between child components

**Props:** None (route-level component)

**Key Features:**
- Uses `useJiraConfigState()` to access shared state
- Uses `useJiraConnection()` for connection operations
- Uses `useJiraProjects()` for project operations
- Implements adaptive rendering based on `hasConfig` flag
- Wraps content in `<el-card>` with proper loading states

**UI States:**
1. **Empty State** (no config): Shows `JiraConnectionForm`
2. **Connected State** (has config): Shows `JiraConnectionInfo` + `JiraProjectSettings`
3. **Edit Mode** (admin editing): Shows `JiraConnectionForm` with existing data
4. **Loading State**: Shows skeleton/spinner during operations

**Template Structure:**
```vue
<template>
  <div class="jira-config-container">
    <el-card v-loading="loading">
      <!-- Empty State -->
      <JiraConnectionForm v-if="!hasConfig" />
      
      <!-- Connected State -->
      <div v-else>
        <JiraConnectionInfo />
        <JiraProjectSettings />
      </div>
    </el-card>
  </div>
</template>
```

---

### 3.2 JiraConnectionForm.vue

**Location:** `views/Admin/JiraConfig/components/JiraConnectionForm.vue`

**Responsibilities:**
- Form for entering Jira credentials (URL, email, API token)
- Validation with Element Plus rules
- Test connection before save
- Save configuration
- Support both Create and Edit modes

**Props:**
```typescript
interface Props {
  initialData?: JiraConfig | null  // For edit mode
  mode?: 'create' | 'edit'
}
```

**Emits:**
```typescript
interface Emits {
  (e: 'saved'): void      // After successful save
  (e: 'cancelled'): void  // If user cancels edit mode
}
```

**Key Features:**
- Form fields: `jiraUrl`, `jiraEmail`, `jiraApiToken`
- Validation rules (required, URL format, email format)
- "Test Connection" button (calls API, shows success/error message)
- "Save" button (disabled until test passes or user confirms)
- "Cancel" button (for edit mode only)
- Password field for API token with show/hide toggle
- Help text explaining how to generate Jira API token

**Form Fields:**
```typescript
interface FormData {
  jiraUrl: string        // e.g., https://yourcompany.atlassian.net
  jiraEmail: string      // User's Jira email
  jiraApiToken: string   // User's Jira API token
}
```

**Validation Rules:**
- `jiraUrl`: Required, valid URL format, must start with `https://`
- `jiraEmail`: Required, valid email format
- `jiraApiToken`: Required, min 8 characters (Jira tokens vary, use reasonable minimum)

**Note on API Token Validation:**
- Jira Cloud API tokens are typically 24-40 characters (alphanumeric)
- Jira Data Center tokens may vary
- Client-side validation uses min 8 chars to avoid false negatives
- Server-side validation will verify token authenticity on save/test

---

### 3.3 JiraConnectionInfo.vue

**Location:** `views/Admin/JiraConfig/components/JiraConnectionInfo.vue`

**Responsibilities:**
- Display readonly connection information
- Show Jira instance URL, connected email, connection status
- Provide Edit and Disconnect buttons (admin only)
- Display "Test Connection" button for both admin and regular users

**Props:** None (uses shared state via `useJiraConfigState()`)

**Emits:**
```typescript
interface Emits {
  (e: 'edit'): void        // User clicks Edit button
  (e: 'disconnect'): void  // User confirms disconnect
}
```

**Key Features:**
- Readonly fields display with `<el-descriptions>`
- `JiraStatusBadge` component showing connection status
- "Test Connection" button (available to all users)
- "Edit" button (admin only, switches to edit mode)
- "Disconnect" button (admin only, shows confirmation dialog)
- Last tested timestamp display
- Last synced timestamp display

**Display Fields:**
- Jira Instance URL (link)
- Connected Email (masked partially)
- Connection Status (badge)
- Last Tested (timestamp)
- Last Synced (timestamp)
- Connected Since (date)

---

### 3.4 JiraProjectSettings.vue

**Location:** `views/Admin/JiraConfig/components/JiraProjectSettings.vue`

**Responsibilities:**
- Display available Jira projects
- Allow setting default project
- Show Jira metadata (issue types, priorities, statuses)
- Refresh projects list

**Props:** None (uses shared state)

**Key Features:**
- "Fetch Projects" button (calls API, populates project list)
- `JiraProjectSelector` component for default project
- Table/List showing all available projects with details
- Expandable metadata section showing issue types, priorities, statuses
- Loading state during fetch operations
- Empty state if no projects fetched

**Project Display:**
- Project Key
- Project Name
- Project Type
- Lead Name
- Default marker (if default project)

**Metadata Display:**
- Issue Types (list with icons)
- Priorities (list with colors)
- Statuses (list with status category)

---

### 3.5 JiraStatusBadge.vue (Reusable)

**Location:** `components/jira/JiraStatusBadge.vue`

**Responsibilities:**
- Display connection status with appropriate color and icon
- Support multiple status types

**Props:**
```typescript
interface Props {
  status: 'connected' | 'disconnected' | 'testing' | 'error'
  size?: 'small' | 'default' | 'large'
}
```

**Key Features:**
- Uses `<el-tag>` with appropriate type (success/info/warning/danger)
- Icon mapping: connected → CheckIcon, disconnected → InfoIcon, testing → RefreshIcon, error → ErrorIcon
- Color mapping: connected → green, disconnected → gray, testing → blue, error → red

---

### 3.6 JiraProjectSelector.vue (Reusable)

**Location:** `components/jira/JiraProjectSelector.vue`

**Responsibilities:**
- Dropdown selector for Jira projects
- Display project key and name
- Support default selection

**Props:**
```typescript
interface Props {
  projects: JiraProject[]
  modelValue: string | null  // Selected project key
  disabled?: boolean
  placeholder?: string
}
```

**Emits:**
```typescript
interface Emits {
  (e: 'update:modelValue', value: string | null): void
}
```

**Key Features:**
- Uses `<el-select>` with v-model
- Option template showing project key + name
- Search/filter capability
- Empty state if no projects available

---

## 4. Composables Design

### 4.1 useJiraConfigState.ts (Shared State)

**Location:** `composables/jira/useJiraConfigState.ts`

**Purpose:** Singleton reactive state shared across all components

**Rationale for Singleton Pattern:**
- Global Jira config is single-instance (not per-component)
- Multiple components need to read/write same state (JiraConfig, JiraConnectionForm, JiraProjectSettings)
- Avoids prop-drilling through component hierarchy
- Different from `useSidebar.ts` because sidebar state is scoped to layout tree (uses provide/inject for isolation)
- Jira config is application-level singleton state (true global)

**State:**
```typescript
interface JiraConfigState {
  config: Ref<JiraConfig | null>
  loading: Ref<boolean>
  error: Ref<string | null>
  projects: Ref<JiraProject[]>
  metadata: Ref<JiraMetadata | null>
  hasConfig: ComputedRef<boolean>
}
```

**Implementation Pattern:**
```typescript
// Singleton state outside composable
const state = reactive({
  config: null as JiraConfig | null,
  loading: false,
  error: null as string | null,
  projects: [] as JiraProject[],
  metadata: null as JiraMetadata | null,
})

export function useJiraConfigState() {
  return {
    config: toRef(state, 'config'),
    loading: toRef(state, 'loading'),
    error: toRef(state, 'error'),
    projects: toRef(state, 'projects'),
    metadata: toRef(state, 'metadata'),
    hasConfig: computed(() => state.config !== null),
  }
}
```

---

### 4.2 useJiraConnection.ts

**Location:** `composables/jira/useJiraConnection.ts`

**Purpose:** Connection management operations (fetch, save, test, delete)

**Dependencies:**
- `useJiraConfigState()` for shared state
- `jiraConfigApi` for API calls
- `isSuccess()` type guard from `@/types/common`
- `ElNotification`, `ElMessageBox` from Element Plus

**API:**
```typescript
interface UseJiraConnection {
  fetchConfig: () => Promise<void>
  saveConfig: (data: SaveConfigRequest) => Promise<boolean>
  testConnection: (data: TestConnectionRequest) => Promise<boolean>
  deleteConfig: () => Promise<boolean>
}
```

**Implementation Details:**
- All methods use Result<T> pattern from API layer
- Success: Update shared state, show success message
- Failure: Set error in state, show error notification
- Loading state managed automatically

**fetchConfig():**
```typescript
import { isSuccess } from '@/types/common'

async function fetchConfig() {
  loading.value = true
  error.value = null
  
  const result = await jiraConfigApi.getConfig()
  
  if (isSuccess(result)) {
    config.value = result.data
  } else {
    error.value = result.error
    // Don't show notification if config doesn't exist yet (expected)
    // Only show error for unexpected failures
    if (result.error !== 'Configuration not found') {
      ElNotification.error({ title: 'Error', message: result.error })
    }
  }
  
  loading.value = false
}
```

**saveConfig():**
```typescript
async function saveConfig(data: SaveConfigRequest): Promise<boolean> {
  loading.value = true
  error.value = null
  
  const result = await jiraConfigApi.saveConfig(data)
  
  loading.value = false
  
  if (isSuccess(result)) {
    config.value = result.data
    ElNotification.success({ title: 'Success', message: 'Configuration saved' })
    return true
  } else {
    error.value = result.error
    ElNotification.error({ title: 'Error', message: result.error })
    return false
  }
}
```

**testConnection():**
```typescript
async function testConnection(data: TestConnectionRequest): Promise<boolean> {
  loading.value = true
  
  const result = await jiraConfigApi.testConnection(data)
  
  loading.value = false
  
  if (isSuccess(result)) {
    ElNotification.success({ 
      title: 'Success', 
      message: result.data.message || 'Connection successful' 
    })
    return true
  } else {
    ElNotification.error({ title: 'Connection Failed', message: result.error })
    return false
  }
}
```

**deleteConfig():**
```typescript
async function deleteConfig(): Promise<boolean> {
  try {
    await ElMessageBox.confirm(
      'Are you sure you want to disconnect from Jira? This will remove all Jira configuration.',
      'Confirm Disconnect',
      { type: 'warning', confirmButtonText: 'Disconnect', cancelButtonText: 'Cancel' }
    )
  } catch {
    // User cancelled
    return false
  }
  
  loading.value = true
  
  const result = await jiraConfigApi.deleteConfig()
  
  loading.value = false
  
  if (isSuccess(result)) {
    config.value = null
    projects.value = []
    metadata.value = null
    ElNotification.success({ title: 'Success', message: 'Disconnected from Jira' })
    return true
  } else {
    error.value = result.error
    ElNotification.error({ title: 'Error', message: result.error })
    return false
  }
}
```

---

### 4.3 useJiraProjects.ts

**Location:** `composables/jira/useJiraProjects.ts`

**Purpose:** Project fetching and default project management

**Dependencies:**
- `useJiraConfigState()` for shared state
- `jiraConfigApi` for API calls
- `isSuccess()` type guard from `@/types/common`
- `ElNotification` from Element Plus

**API:**
```typescript
interface UseJiraProjects {
  fetchProjects: () => Promise<boolean>
  setDefaultProject: (projectKey: string) => Promise<boolean>
  getDefaultProject: ComputedRef<JiraProject | null>
}
```

**fetchProjects():**
```typescript
import { isSuccess } from '@/types/common'

async function fetchProjects(): Promise<boolean> {
  if (!config.value) {
    ElNotification.warning({ title: 'Warning', message: 'No Jira configuration found' })
    return false
  }
  
  loading.value = true
  
  const result = await jiraConfigApi.fetchProjects()
  
  loading.value = false
  
  if (isSuccess(result)) {
    projects.value = result.data.projects
    metadata.value = result.data.metadata
    ElNotification.success({ 
      title: 'Success', 
      message: `Fetched ${result.data.projects.length} projects` 
    })
    return true
  } else {
    error.value = result.error
    ElNotification.error({ title: 'Error', message: result.error })
    return false
  }
}
```

**setDefaultProject():**
```typescript
async function setDefaultProject(projectKey: string): Promise<boolean> {
  loading.value = true
  
  const result = await jiraConfigApi.setDefaultProject(projectKey)
  
  loading.value = false
  
  if (isSuccess(result)) {
    config.value = result.data
    ElNotification.success({ 
      title: 'Success', 
      message: 'Default project updated' 
    })
    return true
  } else {
    error.value = result.error
    ElNotification.error({ title: 'Error', message: result.error })
    return false
  }
}
```

**getDefaultProject:**
```typescript
const getDefaultProject = computed(() => {
  if (!config.value?.defaultProjectKey) return null
  return projects.value.find(p => p.key === config.value!.defaultProjectKey) || null
})
```

---

## 5. API Layer

### 5.1 Configuration API Service

**Location:** `services/jira/config-api.ts`

**Purpose:** API endpoints for Jira configuration management

**Implementation:**
```typescript
import { apiCall } from '@/utils/api'
import type { ApiResponse } from '@/types/common'
import type {
  JiraConfig,
  SaveConfigRequest,
  TestConnectionRequest,
  TestConnectionResponse,
  FetchProjectsResponse,
} from '@/types/jira/config'

export const jiraConfigApi = {
  /**
   * Get current Jira configuration
   * GET /api/jira/config
   */
  async getConfig(): Promise<ApiResponse<JiraConfig>> {
    return apiCall<JiraConfig>('get', '/api/jira/config')
  },

  /**
   * Save Jira configuration (create or update)
   * POST /api/jira/config
   */
  async saveConfig(data: SaveConfigRequest): Promise<ApiResponse<JiraConfig>> {
    return apiCall<JiraConfig, SaveConfigRequest>('post', '/api/jira/config', data)
  },

  /**
   * Test Jira connection with provided credentials
   * POST /api/jira/config/test
   */
  async testConnection(data: TestConnectionRequest): Promise<ApiResponse<TestConnectionResponse>> {
    return apiCall<TestConnectionResponse, TestConnectionRequest>('post', '/api/jira/config/test', data)
  },

  /**
   * Delete Jira configuration
   * DELETE /api/jira/config
   */
  async deleteConfig(): Promise<ApiResponse<void>> {
    return apiCall<void>('delete', '/api/jira/config')
  },

  /**
   * Fetch available projects from Jira
   * GET /api/jira/config/projects
   */
  async fetchProjects(): Promise<ApiResponse<FetchProjectsResponse>> {
    return apiCall<FetchProjectsResponse>('get', '/api/jira/config/projects')
  },

  /**
   * Set default project for workspace
   * PUT /api/jira/config/default-project
   */
  async setDefaultProject(projectKey: string): Promise<ApiResponse<JiraConfig>> {
    return apiCall<JiraConfig, { projectKey: string }>('put', '/api/jira/config/default-project', { projectKey })
  },
}
```

---

## 6. Type Definitions

### 6.1 Core Configuration Types

**Location:** `types/jira/config.ts`

```typescript
/**
 * Jira configuration stored in backend
 */
export interface JiraConfig {
  id: string
  jiraUrl: string
  jiraEmail: string
  isConnected: boolean
  lastTested: string | null  // ISO 8601 timestamp
  lastSynced: string | null  // ISO 8601 timestamp
  createdAt: string          // ISO 8601 timestamp
  defaultProjectKey: string | null
}

/**
 * Request payload for saving Jira configuration
 */
export interface SaveConfigRequest {
  jiraUrl: string
  jiraEmail: string
  jiraApiToken: string
}

/**
 * Request payload for testing connection
 */
export interface TestConnectionRequest {
  jiraUrl: string
  jiraEmail: string
  jiraApiToken: string
}

/**
 * Response from test connection endpoint
 */
export interface TestConnectionResponse {
  success: boolean
  message: string
  jiraInstanceInfo?: {
    baseUrl: string
    version: string
    serverTitle: string
  }
}

/**
 * Jira project information
 */
export interface JiraProject {
  key: string
  name: string
  projectTypeKey: string  // e.g., 'software', 'business'
  lead: {
    accountId: string
    displayName: string
    emailAddress: string
  }
  avatarUrl?: string
  description?: string
}

/**
 * Jira metadata (issue types, priorities, statuses)
 */
export interface JiraMetadata {
  issueTypes: JiraIssueType[]
  priorities: JiraPriority[]
  statuses: JiraStatus[]
}

export interface JiraIssueType {
  id: string
  name: string
  description: string
  iconUrl: string
  subtask: boolean
}

export interface JiraPriority {
  id: string
  name: string
  description: string
  iconUrl: string
  statusColor: string  // e.g., '#FF0000'
}

export interface JiraStatus {
  id: string
  name: string
  description: string
  statusCategory: {
    key: string           // e.g., 'new', 'indeterminate', 'done'
    name: string
    colorName: string     // e.g., 'blue-gray', 'yellow', 'green'
  }
}

/**
 * Response from fetch projects endpoint
 */
export interface FetchProjectsResponse {
  projects: JiraProject[]
  metadata: JiraMetadata
}
```

---

## 7. UI Flow & State Transitions

### 7.1 Initial Load Flow

```
User navigates to /admin/jira-config
  ↓
JiraConfig.vue: onMounted() → fetchConfig()
  ↓
Check Result<JiraConfig>
  ├─ Success (config exists) → Show Connected State
  │    ├─ Display JiraConnectionInfo
  │    └─ Display JiraProjectSettings
  │
  └─ Failure (404 - no config) → Show Empty State
       └─ Display JiraConnectionForm
```

### 7.2 New Connection Flow

```
Empty State: User fills form
  ↓
User clicks "Test Connection"
  ↓
testConnection() → POST /api/jira/config/test
  ├─ Success → Enable "Save" button
  └─ Failure → Show error, disable "Save"
  ↓
User clicks "Save"
  ↓
saveConfig() → POST /api/jira/config
  ├─ Success → config.value = result.data
  │              UI transitions to Connected State
  └─ Failure → Show error, stay in form
```

### 7.3 Edit Connection Flow

```
Connected State: Admin clicks "Edit"
  ↓
JiraConfig.vue emits 'edit' → Switch to edit mode
  ↓
Render JiraConnectionForm with initialData (pre-filled)
  ↓
User modifies fields + tests + saves
  ↓
saveConfig() → POST /api/jira/config
  ├─ Success → Update config.value
  │              UI returns to Connected State
  └─ Failure → Show error, stay in form
  ↓
If user clicks "Cancel" → Return to Connected State without saving
```

### 7.4 Disconnect Flow

```
Connected State: Admin clicks "Disconnect"
  ↓
Show confirmation dialog (ElMessageBox)
  ├─ Confirmed → deleteConfig() → DELETE /api/jira/config
  │    ├─ Success → config.value = null
  │    │              projects.value = []
  │    │              metadata.value = null
  │    │              UI transitions to Empty State
  │    └─ Failure → Show error, stay in Connected State
  │
  └─ Cancelled → No action, stay in Connected State
```

### 7.5 Fetch Projects Flow

```
Connected State: User clicks "Fetch Projects"
  ↓
fetchProjects() → GET /api/jira/config/projects
  ├─ Success → projects.value = result.data.projects
  │              metadata.value = result.data.metadata
  │              Update JiraProjectSettings display
  └─ Failure → Show error notification
  ↓
Admin selects default project from dropdown
  ↓
setDefaultProject(projectKey) → PUT /api/jira/config/default-project
  ├─ Success → config.value.defaultProjectKey = projectKey
  │              Update UI to reflect default
  └─ Failure → Show error notification
```

---

## 8. Permission Model

### 8.1 Permission Levels

| User Role | View Config | Edit Config | Delete Config | Test Connection | Fetch Projects |
|-----------|-------------|-------------|---------------|-----------------|----------------|
| Admin     | ✅          | ✅          | ✅            | ✅              | ✅             |
| User      | ✅          | ❌          | ❌            | ✅              | ✅             |
| Guest     | ❌          | ❌          | ❌            | ❌              | ❌             |

### 8.2 UI Conditional Rendering

```typescript
// In JiraConfig.vue
const isAdmin = computed(() => {
  // Phase 1: Mock admin (always true for testing)
  // TODO: Replace with actual auth check when user store is implemented
  // return userStore.hasRole('Admin')
  return true
})

// In JiraConnectionInfo.vue
<el-button v-if="isAdmin" @click="handleEdit">Edit</el-button>
<el-button v-if="isAdmin" @click="handleDisconnect">Disconnect</el-button>
<el-button @click="handleTest">Test Connection</el-button>
```

### 8.3 API Authorization

Backend enforces permissions:
- `POST /api/jira/config` → Admin only
- `DELETE /api/jira/config` → Admin only
- `PUT /api/jira/config/default-project` → Admin only
- `GET /api/jira/config` → Authenticated users
- `POST /api/jira/config/test` → Authenticated users
- `GET /api/jira/config/projects` → Authenticated users

Frontend shows appropriate UI, but backend validates all write operations.

---

## 9. Error Handling Strategy

### 9.1 Network Errors

- No internet connection → Show notification "Check your network connection"
- Timeout → Show notification "Request timed out. Please try again"
- 5xx errors → Show notification "Server error. Please contact support"

### 9.2 Validation Errors

- Form validation → Show inline error below field (Element Plus validation)
- 400 Bad Request → Show notification with backend error message
- 422 Unprocessable Entity → Show field-specific errors if provided

### 9.3 Authorization Errors

- 401 Unauthorized → Redirect to login page
- 403 Forbidden → Show notification "You don't have permission to perform this action"

### 9.4 Business Logic Errors

- Jira connection failed → Show notification with reason (invalid credentials, unreachable server, etc.)
- Config already exists (on duplicate save) → Backend returns 409 Conflict, show notification "Configuration already exists. Use Edit instead."
- No projects found → Show empty state in JiraProjectSettings with helpful message
- Invalid Jira URL format → Backend returns 400 Bad Request, show notification with validation error
- Jira API rate limit exceeded → Backend returns 429 Too Many Requests, show notification "Rate limit exceeded. Please try again later."

**Expected Backend Error Codes:**
- `400 Bad Request` - Validation errors (invalid URL, missing fields)
- `401 Unauthorized` - User not authenticated
- `403 Forbidden` - User lacks Admin role for write operations
- `404 Not Found` - No configuration exists (expected on first load)
- `409 Conflict` - Configuration already exists (duplicate save attempt)
- `429 Too Many Requests` - Jira API rate limit
- `500 Internal Server Error` - Unexpected server failure
- `502 Bad Gateway` - Cannot reach Jira instance
- `503 Service Unavailable` - Jira service temporarily down

### 9.5 Error Display Components

```typescript
import { isSuccess } from '@/types/common'

// In composables
if (!isSuccess(result)) {
  // Use Element Plus notification
  ElNotification.error({
    title: 'Error',
    message: result.error,
    duration: 5000,
  })
  
  // Also set in state for component-level display
  error.value = result.error
}
```

---

## 10. Success Criteria

### 10.1 Functional Requirements

- ✅ Admin can create new Jira configuration
- ✅ Admin can test connection before saving
- ✅ Admin can edit existing configuration
- ✅ Admin can delete (disconnect) configuration
- ✅ All users can view connection status
- ✅ All users can test connection (read-only verification)
- ✅ Admin can fetch available projects from Jira
- ✅ Admin can set default project
- ✅ UI shows appropriate buttons based on permissions
- ✅ UI adapts between Empty State and Connected State
- ✅ All API calls follow ApiResponse<T> pattern with isSuccess() type guard
- ✅ All state is shared via singleton composables

### 10.2 Non-Functional Requirements

- ✅ Type-safe TypeScript throughout (no `any`)
- ✅ Consistent error handling with notifications
- ✅ Responsive UI (mobile-friendly)
- ✅ Loading states during async operations
- ✅ Form validation with clear error messages
- ✅ Confirmation dialogs for destructive actions
- ✅ Accessibility (ARIA labels, keyboard navigation)

### 10.3 Code Quality

- ✅ No duplicate code (DRY principle)
- ✅ Separation of concerns (UI ↔ Logic ↔ API)
- ✅ Composables follow Vue 3 best practices
- ✅ Components are single-responsibility
- ✅ Reusable components extracted (JiraStatusBadge, JiraProjectSelector)
- ✅ JSDoc comments on all public APIs

### 10.4 Testing Plan

**Unit Tests:**
- Composables: `useJiraConnection`, `useJiraProjects`, `useJiraConfigState`
- API service: `jiraConfigApi` (mock axios)

**Component Tests:**
- Form validation in `JiraConnectionForm`
- Permission-based rendering in `JiraConnectionInfo`
- Project selection in `JiraProjectSelector`

**Integration Tests:**
- Full flow: Empty State → Save Config → Connected State
- Edit flow: Connected State → Edit Mode → Save → Connected State
- Delete flow: Connected State → Disconnect → Empty State

---

## 11. Implementation Notes

### 11.1 Element Plus Components Used

- `<el-card>` - Main container
- `<el-form>` / `<el-form-item>` - Forms
- `<el-input>` - Text inputs
- `<el-button>` - Actions
- `<el-descriptions>` - Readonly info display
- `<el-tag>` - Status badges
- `<el-select>` - Project selector
- `<el-table>` - Projects list (optional)
- `<el-notification>` - Success/error messages
- `<el-message-box>` - Confirmation dialogs
- `v-loading` - Loading states

### 11.2 Route Configuration

Add to router:
```typescript
{
  path: '/admin/jira-config',
  name: 'JiraConfig',
  component: () => import('@/views/Admin/JiraConfig/JiraConfig.vue'),
  meta: { 
    requiresAuth: true,
    title: 'Jira Configuration'
  }
}
```

### 11.3 Backend Endpoints Required

These endpoints must be implemented in the backend (WorkHub.API):

1. `GET /api/jira/config` - Returns current configuration or 404
2. `POST /api/jira/config` - Creates/updates configuration
3. `POST /api/jira/config/test` - Tests connection without saving
4. `DELETE /api/jira/config` - Deletes configuration
5. `GET /api/jira/config/projects` - Fetches projects + metadata from Jira
6. `PUT /api/jira/config/default-project` - Sets default project

All endpoints should return responses that can be converted to ApiResponse<T> format by the shared apiCall() utility.

### 11.4 Dependencies

No new dependencies required. Uses existing:
- Vue 3
- TypeScript
- Element Plus
- Axios (via shared `apiCall` utility)

### 11.5 Styling Guidelines

- Follow existing WorkHub design system
- Use Element Plus CSS variables for theming
- Responsive breakpoints: mobile (<768px), tablet (768-1024px), desktop (>1024px)
- Consistent spacing: 16px base unit
- Form max-width: 600px

---

## 12. Future Enhancements (Out of Scope)

These features are explicitly deferred to future phases:

### Phase 2:
- Organization-level configuration (multiple Jira instances per org)
- Workspace-level configuration override
- User-specific Jira credentials (for personal tasks)
- Advanced project filtering and searching

### Phase 3:
- Custom field mapping configuration
- Webhook setup for real-time sync
- Advanced authentication (OAuth 2.0, SSO)
- Jira webhook management UI
- Custom workflows and automation rules

---

## 13. Approval Checklist

Before implementation begins, verify:

**Design Approvals:**
- ✅ Design Section 1: File structure approved
- ✅ Design Section 2: Component architecture approved
- ✅ Design Section 3: Composables & state management approved
- ✅ Design Section 4: API layer & types approved

**Spec Quality:**
- ✅ Critical issues fixed (ApiResponse<T> pattern, apiCall() signatures, isSuccess() usage)
- ✅ Loading state management corrected (moved to end of functions)
- ✅ Error handling patterns documented
- ⏳ Spec document reviewed by user

**Prerequisites (see Section 1.5):**
- ⚠️ Backend endpoints availability must be confirmed
- ⚠️ Permission strategy decided (mock for Phase 1 or implement full auth)
- ✅ Shared infrastructure available (apiCall, types, Element Plus)

**Next Steps:**
- ⏳ User final review & approval
- ⏳ Implementation plan created (writing-plans skill)
- ⏳ Backend coordination (if endpoints don't exist)

---

## 14. References

- [Jira REST API Documentation](https://developer.atlassian.com/cloud/jira/platform/rest/v3/intro/)
- [Element Plus Form Documentation](https://element-plus.org/en-US/component/form.html)
- [Vue 3 Composition API](https://vuejs.org/guide/extras/composition-api-faq.html)
- WorkHub Frontend: [README.md](../../FrontEnd/README.md)
- Shared API Utility: [src/utils/api.ts](../../FrontEnd/src/utils/api.ts)
- ApiResponse<T> Pattern: [src/types/common.ts](../../FrontEnd/src/types/common.ts)

---

**Spec Status:** Reviewed & corrected (Critical issues fixed)  
**Remaining Work:**
1. User final approval of spec
2. Confirm backend endpoints availability (Section 1.5)
3. Decide permission strategy (mock vs. implement auth)
4. Create implementation plan via writing-plans skill

**Next Step:** User review this spec document and approve proceed to writing-plans
