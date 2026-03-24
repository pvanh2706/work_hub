# Jira Configuration UI Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a comprehensive UI for managing Jira connection settings in WorkHub with connection testing, project fetching, and metadata display.

**Architecture:** Vue 3 component-based UI with shared singleton state (composables pattern), Element Plus UI library, ApiResponse<T> pattern for type-safe API calls. Mock admin permissions for Phase 1 (replace with real auth later).

**Tech Stack:** Vue 3, TypeScript, Element Plus, Vite, existing apiCall utility

**Spec Reference:** `docs/superpowers/specs/2026-03-23-jira-config-ui.md`

**Note:** Backend API endpoints are not yet implemented. This plan builds a functional UI that will show API errors until backend is ready. This is expected and does not indicate implementation failure.

---

## File Overview

**Creating 11 new files:**
1. `FrontEnd/src/types/jira/config.ts` - Type definitions
2. `FrontEnd/src/services/jira/config-api.ts` - API service
3. `FrontEnd/src/composables/jira/useJiraConfigState.ts` - Shared state
4. `FrontEnd/src/composables/jira/useJiraConnection.ts` - Connection operations
5. `FrontEnd/src/composables/jira/useJiraProjects.ts` - Project operations
6. `FrontEnd/src/components/jira/JiraStatusBadge.vue` - Status badge component
7. `FrontEnd/src/components/jira/JiraProjectSelector.vue` - Project selector component
8. `FrontEnd/src/views/Admin/JiraConfig/components/JiraConnectionForm.vue` - Credentials form
9. `FrontEnd/src/views/Admin/JiraConfig/components/JiraConnectionInfo.vue` - Connection info display
10. `FrontEnd/src/views/Admin/JiraConfig/components/JiraProjectSettings.vue` - Project settings
11. `FrontEnd/src/views/Admin/JiraConfig/JiraConfig.vue` - Main container

**Modifying 1 file:**
- `FrontEnd/src/router/index.ts` - Add Jira Config route

---

## Task 0: Verify Prerequisites

**Purpose:** Confirm required infrastructure exists before implementation

- [ ] **Step 1: Verify API utilities exist**

```bash
cd FrontEnd
test -f src/utils/api.ts && echo "✓ apiCall utility exists" || echo "✗ Missing: src/utils/api.ts"
test -f src/types/common.ts && echo "✓ Common types exist" || echo "✗ Missing: src/types/common.ts"
```

Expected: Both files exist

- [ ] **Step 2: Verify layout components exist**

```bash
test -f src/components/layout/AdminLayout.vue && echo "✓ AdminLayout exists" || echo "✗ Missing: AdminLayout.vue"
test -f src/components/common/PageBreadcrumb.vue && echo "✓ PageBreadcrumb exists" || echo "✗ Missing: PageBreadcrumb.vue"
```

Expected: Both components exist

- [ ] **Step 3: Verify required icons exist**

```bash
cd src/icons
test -f CheckIcon.vue && echo "✓ CheckIcon exists" || echo "✗ Missing: CheckIcon.vue"
test -f InfoIcon.vue && echo "✓ InfoIcon exists" || echo "✗ Missing: InfoIcon.vue"
test -f RefreshIcon.vue && echo "✓ RefreshIcon exists" || echo "✗ Missing: RefreshIcon.vue"
test -f ErrorIcon.vue && echo "✓ ErrorIcon exists" || echo "✗ Missing: ErrorIcon.vue"
cd ../..
```

Expected: All 4 icons exist

- [ ] **Step 4: Read and verify apiCall signature**

Open `src/utils/api.ts` and confirm signature matches:

```typescript
function apiCall<TResponse, TRequest = unknown>(
  method: 'get' | 'post' | 'put' | 'delete',
  endpoint: string,
  data?: TRequest
): Promise<ApiResponse<TResponse>>
```

Expected: Signature matches (parameter names can vary)

- [ ] **Step 5: Read and verify isSuccess() type guard**

Open `src/types/common.ts` and confirm this function exists:

```typescript
function isSuccess<T>(response: ApiResponse<T>): response is { isSuccess: true; data: T }
```

Expected: Type guard exists (implementation details can vary)

**If any verification fails:**
- Locate correct path/file for existing infrastructure
- Create missing files before proceeding
- Or revise plan to work without missing dependencies

---

## Task 1: Type Definitions

**Files:**
- Create: `FrontEnd/src/types/jira/config.ts`

- [ ] **Step 1: Create types directory**

```bash
mkdir -p FrontEnd/src/types/jira
```

- [ ] **Step 2: Create config.ts with core types**

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

- [ ] **Step 3: Verify TypeScript compilation**

```bash
cd FrontEnd
npm run type-check
```

Expected: No errors in `types/jira/config.ts`

- [ ] **Step 4: Commit**

```bash
git add FrontEnd/src/types/jira/config.ts
git commit -m "feat(jira): add Jira configuration type definitions"
```

---

## Task 2: API Service

**Files:**
- Create: `FrontEnd/src/services/jira/config-api.ts`
- Reference: `FrontEnd/src/utils/api.ts` (apiCall utility)

- [ ] **Step 1: Create config-api.ts**

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

/**
 * Jira Configuration API Service
 * Handles all API calls related to Jira connection configuration
 */
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

- [ ] **Step 2: Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: No errors

- [ ] **Step 3: Commit**

```bash
git add FrontEnd/src/services/jira/config-api.ts
git commit -m "feat(jira): add Jira configuration API service"
```

---

## Task 3: Shared State Composable

**Files:**
- Create: `FrontEnd/src/composables/jira/useJiraConfigState.ts`

- [ ] **Step 1: Create composables directory**

```bash
mkdir -p FrontEnd/src/composables/jira
```

- [ ] **Step 2: Create useJiraConfigState.ts**

```typescript
import { reactive, toRef, computed } from 'vue'
import type { JiraConfig, JiraProject, JiraMetadata } from '@/types/jira/config'

/**
 * Singleton reactive state for Jira configuration
 * Shared across all Jira config components
 */
const state = reactive({
  config: null as JiraConfig | null,
  loading: false,
  error: null as string | null,
  projects: [] as JiraProject[],
  metadata: null as JiraMetadata | null,
})

/**
 * Shared state composable for Jira configuration
 * Uses singleton pattern for application-level state
 */
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

- [ ] **Step 3: Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: No errors

- [ ] **Step 4: Commit**

```bash
git add FrontEnd/src/composables/jira/useJiraConfigState.ts
git commit -m "feat(jira): add shared state composable for Jira config"
```

---

## Task 4: Connection Operations Composable

**Files:**
- Create: `FrontEnd/src/composables/jira/useJiraConnection.ts`

- [ ] **Step 1: Create useJiraConnection.ts**

```typescript
import { ElNotification, ElMessageBox } from 'element-plus'
import { useJiraConfigState } from './useJiraConfigState'
import { jiraConfigApi } from '@/services/jira/config-api'
import { isSuccess } from '@/types/common'
import type { SaveConfigRequest, TestConnectionRequest } from '@/types/jira/config'

/**
 * Composable for Jira connection operations
 * Handles fetching, saving, testing, and deleting Jira configuration
 */
export function useJiraConnection() {
  const { config, loading, error, projects, metadata } = useJiraConfigState()

  /**
   * Fetch current Jira configuration from backend
   */
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

  /**
   * Save Jira configuration (create or update)
   */
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

  /**
   * Test Jira connection with provided credentials
   */
  async function testConnection(data: TestConnectionRequest): Promise<boolean> {
    loading.value = true

    const result = await jiraConfigApi.testConnection(data)

    loading.value = false

    if (isSuccess(result)) {
      ElNotification.success({
        title: 'Success',
        message: result.data.message || 'Connection successful',
      })
      return true
    } else {
      ElNotification.error({ title: 'Connection Failed', message: result.error })
      return false
    }
  }

  /**
   * Delete Jira configuration
   */
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

  return {
    fetchConfig,
    saveConfig,
    testConnection,
    deleteConfig,
  }
}
```

- [ ] **Step 2: Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: No errors

- [ ] **Step 3: Commit**

```bash
git add FrontEnd/src/composables/jira/useJiraConnection.ts
git commit -m "feat(jira): add connection operations composable"
```

---

## Task 5: Project Operations Composable

**Files:**
- Create: `FrontEnd/src/composables/jira/useJiraProjects.ts`

- [ ] **Step 1: Create useJiraProjects.ts**

```typescript
import { computed } from 'vue'
import { ElNotification } from 'element-plus'
import { useJiraConfigState } from './useJiraConfigState'
import { jiraConfigApi } from '@/services/jira/config-api'
import { isSuccess } from '@/types/common'

/**
 * Composable for Jira project operations
 * Handles fetching projects and setting default project
 */
export function useJiraProjects() {
  const { config, loading, error, projects, metadata } = useJiraConfigState()

  /**
   * Fetch available projects from Jira
   */
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
        message: `Fetched ${result.data.projects.length} projects`,
      })
      return true
    } else {
      error.value = result.error
      ElNotification.error({ title: 'Error', message: result.error })
      return false
    }
  }

  /**
   * Set default project for workspace
   */
  async function setDefaultProject(projectKey: string): Promise<boolean> {
    loading.value = true

    const result = await jiraConfigApi.setDefaultProject(projectKey)

    loading.value = false

    if (isSuccess(result)) {
      config.value = result.data
      ElNotification.success({
        title: 'Success',
        message: 'Default project updated',
      })
      return true
    } else {
      error.value = result.error
      ElNotification.error({ title: 'Error', message: result.error })
      return false
    }
  }

  /**
   * Get current default project object
   */
  const defaultProject = computed(() => {
    if (!config.value?.defaultProjectKey) return null
    return projects.value.find((p) => p.key === config.value!.defaultProjectKey) || null
  })

  return {
    fetchProjects,
    setDefaultProject,
    defaultProject,
  }
}
```

- [ ] **Step 2: Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: No errors

- [ ] **Step 3: Commit**

```bash
git add FrontEnd/src/composables/jira/useJiraProjects.ts
git commit -m "feat(jira): add project operations composable"
```

---

## Task 6: Jira Status Badge Component

**Files:**
- Create: `FrontEnd/src/components/jira/JiraStatusBadge.vue`

- [ ] **Step 1: Create components directory**

```bash
mkdir -p FrontEnd/src/components/jira
```

- [ ] **Step 2: Create JiraStatusBadge.vue**

```vue
<template>
  <el-tag :type="tagType" :size="size">
    <component :is="iconComponent" class="mr-1" style="width: 14px; height: 14px" />
    {{ statusText }}
  </el-tag>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { CheckIcon, InfoIcon, RefreshIcon, ErrorIcon } from '@/icons'

interface Props {
  status: 'connected' | 'disconnected' | 'testing' | 'error'
  size?: 'small' | 'default' | 'large'
}

const props = withDefaults(defineProps<Props>(), {
  size: 'default',
})

const tagType = computed(() => {
  switch (props.status) {
    case 'connected':
      return 'success'
    case 'disconnected':
      return 'info'
    case 'testing':
      return 'warning'
    case 'error':
      return 'danger'
    default:
      return 'info'
  }
})

const iconComponent = computed(() => {
  switch (props.status) {
    case 'connected':
      return CheckIcon
    case 'disconnected':
      return InfoIcon
    case 'testing':
      return RefreshIcon
    case 'error':
      return ErrorIcon
    default:
      return InfoIcon
  }
})

const statusText = computed(() => {
  switch (props.status) {
    case 'connected':
      return 'Connected'
    case 'disconnected':
      return 'Not Connected'
    case 'testing':
      return 'Testing...'
    case 'error':
      return 'Error'
    default:
      return 'Unknown'
  }
})
</script>
```

- [ ] **Step 3: Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: No errors

- [ ] **Step 4: Commit**

```bash
git add FrontEnd/src/components/jira/JiraStatusBadge.vue
git commit -m "feat(jira): add status badge component"
```

---

## Task 7: Jira Project Selector Component

**Files:**
- Create: `FrontEnd/src/components/jira/JiraProjectSelector.vue`

- [ ] **Step 1: Create JiraProjectSelector.vue**

```vue
<template>
  <el-select
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    :placeholder="placeholder"
    :disabled="disabled"
    filterable
    clearable
    class="w-full"
  >
    <el-option
      v-for="project in projects"
      :key="project.key"
      :label="`${project.key} - ${project.name}`"
      :value="project.key"
    >
      <div class="flex items-center">
        <img v-if="project.avatarUrl" :src="project.avatarUrl" class="w-5 h-5 mr-2 rounded" />
        <span class="font-medium">{{ project.key }}</span>
        <span class="ml-2 text-gray-500">{{ project.name }}</span>
      </div>
    </el-option>
    <template #empty>
      <div class="text-center text-gray-400 py-2">No projects available</div>
    </template>
  </el-select>
</template>

<script setup lang="ts">
import type { JiraProject } from '@/types/jira/config'

interface Props {
  projects: JiraProject[]
  modelValue: string | null
  disabled?: boolean
  placeholder?: string
}

interface Emits {
  (e: 'update:modelValue', value: string | null): void
}

withDefaults(defineProps<Props>(), {
  disabled: false,
  placeholder: 'Select a project',
})

defineEmits<Emits>()
</script>
```

- [ ] **Step 2: Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: No errors

- [ ] **Step 3: Commit**

```bash
git add FrontEnd/src/components/jira/JiraProjectSelector.vue
git commit -m "feat(jira): add project selector component"
```

---

## Task 8: Jira Connection Form Component

**Files:**
- Create: `FrontEnd/src/views/Admin/JiraConfig/components/JiraConnectionForm.vue`

- [ ] **Step 1: Create views directory structure**

```bash
mkdir -p FrontEnd/src/views/Admin/JiraConfig/components
```

- [ ] **Step 2: Create JiraConnectionForm.vue**

```vue
<template>
  <div class="jira-connection-form">
    <h3 class="text-lg font-semibold mb-4 text-gray-800 dark:text-white/90">
      {{ mode === 'edit' ? 'Edit Jira Configuration' : 'Connect to Jira' }}
    </h3>

    <el-form :model="formData" :rules="rules" ref="formRef" label-position="top">
      <el-form-item label="Jira Instance URL" prop="jiraUrl">
        <el-input
          v-model="formData.jiraUrl"
          placeholder="https://yourcompany.atlassian.net"
          :disabled="loading"
        />
        <template #extra>
          <span class="text-sm text-gray-500">Your Jira Cloud or Data Center URL</span>
        </template>
      </el-form-item>

      <el-form-item label="Email Address" prop="jiraEmail">
        <el-input
          v-model="formData.jiraEmail"
          type="email"
          placeholder="your-email@company.com"
          :disabled="loading"
        />
        <template #extra>
          <span class="text-sm text-gray-500">Email used to login to Jira</span>
        </template>
      </el-form-item>

      <el-form-item label="API Token" prop="jiraApiToken">
        <el-input
          v-model="formData.jiraApiToken"
          :type="showPassword ? 'text' : 'password'"
          placeholder="Enter your Jira API token"
          :disabled="loading"
        >
          <template #append>
            <el-button @click="showPassword = !showPassword" :icon="showPassword ? 'View' : 'Hide'">
              {{ showPassword ? 'Hide' : 'Show' }}
            </el-button>
          </template>
        </el-input>
        <template #extra>
          <span class="text-sm text-gray-500">
            <a
              href="https://id.atlassian.com/manage-profile/security/api-tokens"
              target="_blank"
              class="text-blue-500 hover:underline"
            >
              Create API token
            </a>
            in your Atlassian account settings
          </span>
        </template>
      </el-form-item>

      <div class="flex gap-3 mt-6">
        <el-button
          type="primary"
          @click="handleTest"
          :loading="testing"
          :disabled="!isFormValid || loading"
        >
          Test Connection
        </el-button>
        <el-button
          type="success"
          @click="handleSave"
          :loading="loading"
          :disabled="!testPassed && !forceSave"
        >
          Save Configuration
        </el-button>
        <el-button v-if="mode === 'edit'" @click="handleCancel" :disabled="loading">
          Cancel
        </el-button>
      </div>

      <div v-if="testResult" class="mt-4">
        <el-alert
          :type="testResult.success ? 'success' : 'error'"
          :title="testResult.message"
          :closable="false"
        />
      </div>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { useJiraConnection } from '@/composables/jira/useJiraConnection'
import { useJiraConfigState } from '@/composables/jira/useJiraConfigState'
import type { JiraConfig } from '@/types/jira/config'

interface Props {
  initialData?: JiraConfig | null
  mode?: 'create' | 'edit'
}

interface Emits {
  (e: 'saved'): void
  (e: 'cancelled'): void
}

const props = withDefaults(defineProps<Props>(), {
  mode: 'create',
  initialData: null,
})

const emit = defineEmits<Emits>()

const { saveConfig, testConnection } = useJiraConnection()
const { loading } = useJiraConfigState()

const formRef = ref<FormInstance>()
const showPassword = ref(false)
const testing = ref(false)
const testPassed = ref(false)
const forceSave = ref(false)
const testResult = ref<{ success: boolean; message: string } | null>(null)

const formData = reactive({
  jiraUrl: '',
  jiraEmail: '',
  jiraApiToken: '',
})

const rules: FormRules = {
  jiraUrl: [
    { required: true, message: 'Jira URL is required', trigger: 'blur' },
    { type: 'url', message: 'Must be a valid URL', trigger: 'blur' },
    { pattern: /^https:\/\//, message: 'URL must start with https://', trigger: 'blur' },
  ],
  jiraEmail: [
    { required: true, message: 'Email is required', trigger: 'blur' },
    { type: 'email', message: 'Must be a valid email', trigger: 'blur' },
  ],
  jiraApiToken: [
    { required: true, message: 'API token is required', trigger: 'blur' },
    { min: 8, message: 'API token must be at least 8 characters', trigger: 'blur' },
  ],
}

const isFormValid = computed(() => {
  return formData.jiraUrl && formData.jiraEmail && formData.jiraApiToken.length >= 8
})

async function handleTest() {
  const valid = await formRef.value?.validate()
  if (!valid) return

  testing.value = true
  testResult.value = null

  const success = await testConnection({
    jiraUrl: formData.jiraUrl,
    jiraEmail: formData.jiraEmail,
    jiraApiToken: formData.jiraApiToken,
  })

  testing.value = false
  testPassed.value = success
  testResult.value = {
    success,
    message: success ? 'Connection successful!' : 'Connection failed. Please check your credentials.',
  }
}

async function handleSave() {
  const valid = await formRef.value?.validate()
  if (!valid) return

  const success = await saveConfig({
    jiraUrl: formData.jiraUrl,
    jiraEmail: formData.jiraEmail,
    jiraApiToken: formData.jiraApiToken,
  })

  if (success) {
    emit('saved')
  }
}

function handleCancel() {
  emit('cancelled')
}

onMounted(() => {
  if (props.initialData) {
    formData.jiraUrl = props.initialData.jiraUrl
    formData.jiraEmail = props.initialData.jiraEmail
    // API token is not returned from backend for security
    formData.jiraApiToken = ''
  }
})
</script>
```

- [ ] **Step 3: Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: No errors

- [ ] **Step 4: Commit**

```bash
git add FrontEnd/src/views/Admin/JiraConfig/components/JiraConnectionForm.vue
git commit -m "feat(jira): add connection form component"
```

---

## Task 9: Jira Connection Info Component

**Files:**
- Create: `FrontEnd/src/views/Admin/JiraConfig/components/JiraConnectionInfo.vue`

- [ ] **Step 1: Create JiraConnectionInfo.vue**

```vue
<template>
  <div class="jira-connection-info">
    <div class="flex justify-between items-center mb-4">
      <h3 class="text-lg font-semibold text-gray-800 dark:text-white/90">Jira Connection</h3>
      <JiraStatusBadge :status="connectionStatus" />
    </div>

    <el-descriptions :column="2" border>
      <el-descriptions-item label="Jira Instance">
        <a :href="config?.jiraUrl" target="_blank" class="text-blue-500 hover:underline">
          {{ config?.jiraUrl }}
        </a>
      </el-descriptions-item>
      <el-descriptions-item label="Connected Email">
        {{ config?.jiraEmail }}
      </el-descriptions-item>
      <el-descriptions-item label="Connected Since">
        {{ formatDate(config?.createdAt) }}
      </el-descriptions-item>
      <el-descriptions-item label="Last Tested">
        {{ formatDate(config?.lastTested) || 'Never' }}
      </el-descriptions-item>
      <el-descriptions-item label="Last Synced">
        {{ formatDate(config?.lastSynced) || 'Never' }}
      </el-descriptions-item>
    </el-descriptions>

    <div class="flex gap-3 mt-6">
      <el-button @click="handleTest" :loading="testing">
        Test Connection
      </el-button>
      <el-button v-if="isAdmin" type="primary" @click="$emit('edit')">
        Edit
      </el-button>
      <el-button v-if="isAdmin" type="danger" @click="handleDisconnect" :loading="loading">
        Disconnect
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useJiraConfigState } from '@/composables/jira/useJiraConfigState'
import { useJiraConnection } from '@/composables/jira/useJiraConnection'
import JiraStatusBadge from '@/components/jira/JiraStatusBadge.vue'

interface Emits {
  (e: 'edit'): void
  (e: 'disconnect'): void
}

const emit = defineEmits<Emits>()

const { config, loading } = useJiraConfigState()
const { testConnection, deleteConfig } = useJiraConnection()

const testing = ref(false)

// Phase 1: Mock admin (always true for testing)
// TODO: Replace with actual auth check when user store is implemented
const isAdmin = ref(true)

const connectionStatus = computed(() => {
  if (!config.value) return 'disconnected'
  if (config.value.isConnected) return 'connected'
  return 'error'
})

async function handleTest() {
  if (!config.value) return

  testing.value = true

  await testConnection({
    jiraUrl: config.value.jiraUrl,
    jiraEmail: config.value.jiraEmail,
    jiraApiToken: '', // Backend will use stored token
  })

  testing.value = false
}

async function handleDisconnect() {
  const success = await deleteConfig()
  if (success) {
    emit('disconnect')
  }
}

function formatDate(dateString: string | null | undefined): string {
  if (!dateString) return ''
  return new Date(dateString).toLocaleString()
}
</script>
```

- [ ] **Step 2: Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: No errors

- [ ] **Step 3: Commit**

```bash
git add FrontEnd/src/views/Admin/JiraConfig/components/JiraConnectionInfo.vue
git commit -m "feat(jira): add connection info display component"
```

---

## Task 10: Jira Project Settings Component

**Files:**
- Create: `FrontEnd/src/views/Admin/JiraConfig/components/JiraProjectSettings.vue`

- [ ] **Step 1: Create JiraProjectSettings.vue**

```vue
<template>
  <div class="jira-project-settings mt-6">
    <div class="flex justify-between items-center mb-4">
      <h3 class="text-lg font-semibold text-gray-800 dark:text-white/90">Jira Projects</h3>
      <el-button type="primary" @click="handleFetchProjects" :loading="loading">
        {{ projects.length > 0 ? 'Refresh Projects' : 'Fetch Projects' }}
      </el-button>
    </div>

    <div v-if="projects.length === 0" class="text-center text-gray-400 py-8">
      <p>No projects loaded yet. Click "Fetch Projects" to load available Jira projects.</p>
    </div>

    <div v-else>
      <div class="mb-4">
        <label class="block text-sm font-medium text-gray-700 dark:text-gray-400 mb-2">
          Default Project
        </label>
        <JiraProjectSelector
          :projects="projects"
          :model-value="config?.defaultProjectKey || null"
          @update:model-value="handleSetDefaultProject"
          placeholder="Select default project"
        />
        <span class="text-sm text-gray-500 mt-1 block">
          This project will be used as the default for new tasks
        </span>
      </div>

      <el-table :data="projects" border style="width: 100%">
        <el-table-column prop="key" label="Key" width="120">
          <template #default="{ row }">
            <span class="font-medium">{{ row.key }}</span>
            <el-tag v-if="row.key === config?.defaultProjectKey" type="success" size="small" class="ml-2">
              Default
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="name" label="Name" />
        <el-table-column prop="projectTypeKey" label="Type" width="120" />
        <el-table-column prop="lead.displayName" label="Lead" width="180" />
        <el-table-column label="Avatar" width="80">
          <template #default="{ row }">
            <img v-if="row.avatarUrl" :src="row.avatarUrl" class="w-8 h-8 rounded" />
          </template>
        </el-table-column>
      </el-table>

      <div v-if="metadata" class="mt-6">
        <h4 class="text-md font-semibold mb-3 text-gray-800 dark:text-white/90">Jira Metadata</h4>
        
        <el-collapse>
          <el-collapse-item title="Issue Types" name="issueTypes">
            <div class="flex flex-wrap gap-2">
              <el-tag v-for="type in metadata.issueTypes" :key="type.id">
                <img :src="type.iconUrl" class="w-4 h-4 inline mr-1" />
                {{ type.name }}
              </el-tag>
            </div>
          </el-collapse-item>

          <el-collapse-item title="Priorities" name="priorities">
            <div class="flex flex-wrap gap-2">
              <el-tag v-for="priority in metadata.priorities" :key="priority.id">
                <img :src="priority.iconUrl" class="w-4 h-4 inline mr-1" />
                {{ priority.name }}
              </el-tag>
            </div>
          </el-collapse-item>

          <el-collapse-item title="Statuses" name="statuses">
            <div class="flex flex-wrap gap-2">
              <el-tag
                v-for="status in metadata.statuses"
                :key="status.id"
                :type="getStatusType(status.statusCategory.key)"
              >
                {{ status.name }}
              </el-tag>
            </div>
          </el-collapse-item>
        </el-collapse>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useJiraConfigState } from '@/composables/jira/useJiraConfigState'
import { useJiraProjects } from '@/composables/jira/useJiraProjects'
import JiraProjectSelector from '@/components/jira/JiraProjectSelector.vue'

const { config, loading, projects, metadata } = useJiraConfigState()
const { fetchProjects, setDefaultProject } = useJiraProjects()

async function handleFetchProjects() {
  await fetchProjects()
}

async function handleSetDefaultProject(projectKey: string | null) {
  if (!projectKey) return
  await setDefaultProject(projectKey)
}

function getStatusType(categoryKey: string): 'success' | 'warning' | 'info' | 'danger' {
  switch (categoryKey) {
    case 'done':
      return 'success'
    case 'indeterminate':
      return 'warning'
    case 'new':
      return 'info'
    default:
      return 'info'
  }
}
</script>
```

- [ ] **Step 2: Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: No errors

- [ ] **Step 3: Commit**

```bash
git add FrontEnd/src/views/Admin/JiraConfig/components/JiraProjectSettings.vue
git commit -m "feat(jira): add project settings component"
```

---

## Task 11: Main Jira Config Container

**Files:**
- Create: `FrontEnd/src/views/Admin/JiraConfig/JiraConfig.vue`

- [ ] **Step 1: Create JiraConfig.vue**

```vue
<template>
  <admin-layout>
    <PageBreadcrumb pageTitle="Jira Configuration" />

    <div class="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03] lg:p-6">
      <el-card v-loading="loading">
        <!-- Empty State: No configuration -->
        <div v-if="!hasConfig && !editMode">
          <JiraConnectionForm mode="create" @saved="handleConfigSaved" />
        </div>

        <!-- Connected State: Configuration exists -->
        <div v-else-if="hasConfig && !editMode">
          <JiraConnectionInfo @edit="handleEdit" @disconnect="handleDisconnect" />
          <JiraProjectSettings />
        </div>

        <!-- Edit Mode -->
        <div v-else-if="editMode">
          <JiraConnectionForm
            mode="edit"
            :initial-data="config"
            @saved="handleConfigSaved"
            @cancelled="handleCancelEdit"
          />
        </div>
      </el-card>
    </div>
  </admin-layout>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import AdminLayout from '@/components/layout/AdminLayout.vue'
import PageBreadcrumb from '@/components/common/PageBreadcrumb.vue'
import JiraConnectionForm from './components/JiraConnectionForm.vue'
import JiraConnectionInfo from './components/JiraConnectionInfo.vue'
import JiraProjectSettings from './components/JiraProjectSettings.vue'
import { useJiraConfigState } from '@/composables/jira/useJiraConfigState'
import { useJiraConnection } from '@/composables/jira/useJiraConnection'

const { config, loading, hasConfig } = useJiraConfigState()
const { fetchConfig } = useJiraConnection()

const editMode = ref(false)

function handleEdit() {
  editMode.value = true
}

function handleCancelEdit() {
  editMode.value = false
}

function handleConfigSaved() {
  editMode.value = false
  // Config is already updated in state by useJiraConnection
}

function handleDisconnect() {
  editMode.value = false
  // Config is already cleared in state by useJiraConnection
}

onMounted(async () => {
  await fetchConfig()
})
</script>
```

- [ ] **Step 2: Verify TypeScript compilation**

```bash
npm run type-check
```

Expected: No errors

- [ ] **Step 3: Commit**

```bash
git add FrontEnd/src/views/Admin/JiraConfig/JiraConfig.vue
git commit -m "feat(jira): add main Jira config container component"
```

---

## Task 12: Add Router Configuration

**Files:**
- Modify: `FrontEnd/src/router/index.ts`

- [ ] **Step 1: Add Jira Config route**

Find the routes array and add the new route:

```typescript
{
  path: '/admin/jira-config',
  name: 'JiraConfig',
  component: () => import('../views/Admin/JiraConfig/JiraConfig.vue'),
  meta: {
    title: 'Jira Configuration',
    requiresAuth: true,  // TODO: Enable when auth system is ready
    requiresRole: 'Admin',  // TODO: Enable when auth system is ready
  },
},
```

For now, since auth is not implemented, the route meta is documentation only. When auth system is ready, navigation guards will check these meta fields.

- [ ] **Step 2: Verify router configuration**

```bash
npm run type-check
```

Expected: No errors

- [ ] **Step 3: Test in browser**

Start dev server and navigate to `/admin/jira-config`:

```bash
npm run dev
```

Open: http://localhost:5173/admin/jira-config

Expected: Jira Config UI loads without errors (will show API errors if backend not ready, which is expected)

- [ ] **Step 4: Commit**

```bash
git add FrontEnd/src/router/index.ts
git commit -m "feat(jira): add Jira config route"
```

---

## Completion Checklist

- [ ] All 11 components/files created
- [ ] Router configured
- [ ] TypeScript compilation passes with no errors
- [ ] Dev server runs without errors
- [ ] UI loads at `/admin/jira-config`

## Known Limitations (Phase 1)

1. **Backend API not implemented** - Frontend will show API errors until backend endpoints are created
2. **Mock admin permissions** - All users have admin access (hardcoded `isAdmin = true`)
3. **No authentication** - Route is not protected by auth guards
4. **Test connection without credentials** - Backend needs to store API token securely

## Next Steps (Phase 2)

1. Implement backend API endpoints (`GET/POST/DELETE /api/jira/config`, etc.)
2. Implement user authentication system (Pinia store + role checking)
3. Add navigation guards to protect admin routes
4. Replace mock `isAdmin` with real permission checking
5. Add unit tests for composables
6. Add component tests for forms and displays
