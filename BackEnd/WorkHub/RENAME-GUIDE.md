# Hướng Dẫn Đổi Tên Dự Án Backend (.NET Modular Monolith)

Tài liệu này ghi lại quy trình đổi tên toàn bộ dự án backend từ `DevFlow` sang `WorkHub` và có thể được tái sử dụng cho bất kỳ lần đổi tên nào trong tương lai.

---

## Phạm Vi Thay Đổi

Khi đổi tên một dự án .NET Modular Monolith, cần thay đổi ở **4 cấp độ**:

| Cấp độ | Ví dụ |
|--------|-------|
| Nội dung file (namespace, using, references) | `namespace DevFlow.Shared` → `namespace WorkHub.Shared` |
| Tên file | `DevFlow.API.csproj` → `WorkHub.API.csproj` |
| Tên thư mục con | `DevFlow.Modules.Jira.Application/` → `WorkHub.Modules.Jira.Application/` |
| Thư mục gốc solution | `BackEnd/DevFlow/` → `BackEnd/WorkHub/` |

---

## Các File & Thư Mục Bị Ảnh Hưởng

```
BackEnd/WorkHub/
├── WorkHub.slnx                        ← (đổi tên + nội dung)
├── src/
│   ├── WorkHub.API/                    ← folder + .csproj + content
│   ├── WorkHub.Shared/                 ← folder + .csproj + content
│   └── Modules/
│       ├── AI/
│       │   ├── WorkHub.Modules.AI.Domain/
│       │   ├── WorkHub.Modules.AI.Application/
│       │   └── WorkHub.Modules.AI.Infrastructure/
│       ├── Jira/  (tương tự)
│       ├── Knowledge/  (tương tự)
│       ├── Organization/  (tương tự)
│       ├── Tasks/  (tương tự)
│       └── Workspace/  (tương tự)
└── tests/
    ├── WorkHub.UnitTests/
    └── WorkHub.IntegrationTests/
```

**Tổng cộng (~):**
- 16 file `.csproj` được đổi tên + cập nhật nội dung
- 1 file `.slnx` được đổi tên + cập nhật nội dung
- 22 thư mục được đổi tên
- 50+ file `.cs` được cập nhật namespace/using

---

## Quy Trình Thực Hiện (PowerShell)

### Bước 0 – Chuẩn Bị

Đặt biến `$OLD` và `$NEW` cho toàn bộ phiên:

```powershell
$OLD = "DevFlow"
$NEW = "WorkHub"
$ROOT = "d:\WorkHub\BackEnd\DevFlow"   # đường dẫn gốc solution CŨ
```

---

### Bước 1 – Xóa Build Artifacts

> **Bắt buộc trước khi đổi tên folder** — các file DLL trong `bin/obj` bị khóa bởi VS Code / OmniSharp Language Server, gây lỗi `Access is denied` khi rename.

```powershell
Get-ChildItem -Path $ROOT -Recurse -Directory -Include "bin","obj" |
    Remove-Item -Recurse -Force
Write-Host "Cleaned bin/obj"
```

---

### Bước 2 – Thay Thế Nội Dung Trong Tất Cả File

```powershell
Get-ChildItem -Path $ROOT -Recurse -Include "*.cs","*.csproj","*.slnx","*.json","*.md","*.http","*.txt" |
    ForEach-Object {
        $content = Get-Content $_.FullName -Raw -Encoding UTF8
        if ($content -match $OLD) {
            $newContent = $content -replace $OLD, $NEW
            Set-Content $_.FullName -Value $newContent -Encoding UTF8 -NoNewline
            Write-Host "Updated: $($_.FullName)"
        }
    }
```

Thao tác này cập nhật:
- `namespace DevFlow.*` → `namespace WorkHub.*`
- `using DevFlow.*` → `using WorkHub.*`
- `<ProjectReference Include="...\DevFlow.*.csproj" />` trong các `.csproj`
- Tất cả tham chiếu trong `*.slnx`
- Nội dung `appsettings*.json`, README, và ARCHITECTURE docs

---

### Bước 3 – Đổi Tên Tất Cả File

```powershell
Get-ChildItem -Path $ROOT -Recurse -File |
    Where-Object { $_.Name -like "$OLD*" } |
    ForEach-Object {
        $newName = $_.Name -replace $OLD, $NEW
        Rename-Item -Path $_.FullName -NewName $newName
        Write-Host "Renamed: $($_.Name) -> $newName"
    }
```

---

### Bước 4 – Đổi Tên Tất Cả Thư Mục Con (Từ Sâu Nhất Lên)

> Sort theo độ dài đường dẫn giảm dần để đổi folder con trước, tránh conflict path.

```powershell
Get-ChildItem -Path $ROOT -Recurse -Directory |
    Where-Object { $_.Name -like "$OLD*" } |
    Sort-Object { $_.FullName.Length } -Descending |
    ForEach-Object {
        $newName = $_.Name -replace $OLD, $NEW
        try {
            Rename-Item -Path $_.FullName -NewName $newName -ErrorAction Stop
            Write-Host "OK: $($_.Name) -> $newName"
        } catch {
            Write-Host "FAIL: $($_.Name) - $($_.Exception.Message)"
        }
    }
```

---

### Bước 5 – Đổi Tên Thư Mục Gốc Solution

```powershell
$newRoot = $ROOT -replace $OLD, $NEW
Rename-Item -Path $ROOT -NewName (Split-Path $newRoot -Leaf)
Write-Host "Root renamed: $ROOT -> $newRoot"
```

> **Lưu ý:** Nếu VS Code đang mở workspace với thư mục này, lệnh có thể thất bại với lỗi `Access is denied`.  
> **Giải pháp:** Đóng VS Code hoàn toàn, chạy lệnh trong PowerShell bên ngoài, sau đó mở lại VS Code.

---

### Bước 6 – Cập Nhật File .gitignore (Tùy Chọn)

Nếu `.gitignore` có comment hoặc tên project cứng:

```powershell
$gitignore = "d:\WorkHub\.gitignore"
(Get-Content $gitignore -Raw) -replace $OLD, $NEW | Set-Content $gitignore -Encoding UTF8
```

---

## Script Tổng Hợp (One-Shot)

Lưu file này thành `rename-project.ps1` và chạy khi cần:

```powershell
param(
    [string]$OLD = "DevFlow",
    [string]$NEW  = "WorkHub",
    [string]$ROOT = "d:\WorkHub\BackEnd\DevFlow"
)

# Bước 1: Xóa build artifacts
Write-Host "`n[1/5] Cleaning bin/obj..." -ForegroundColor Cyan
Get-ChildItem -Path $ROOT -Recurse -Directory -Include "bin","obj" | Remove-Item -Recurse -Force

# Bước 2: Thay thế nội dung file
Write-Host "`n[2/5] Replacing content in files..." -ForegroundColor Cyan
Get-ChildItem -Path $ROOT -Recurse -Include "*.cs","*.csproj","*.slnx","*.json","*.md","*.http" |
    ForEach-Object {
        $content = Get-Content $_.FullName -Raw -Encoding UTF8
        if ($content -match $OLD) {
            Set-Content $_.FullName -Value ($content -replace $OLD, $NEW) -Encoding UTF8 -NoNewline
        }
    }

# Bước 3: Đổi tên file
Write-Host "`n[3/5] Renaming files..." -ForegroundColor Cyan
Get-ChildItem -Path $ROOT -Recurse -File | Where-Object { $_.Name -like "$OLD*" } |
    ForEach-Object { Rename-Item $_.FullName -NewName ($_.Name -replace $OLD, $NEW) }

# Bước 4: Đổi tên thư mục con
Write-Host "`n[4/5] Renaming subdirectories..." -ForegroundColor Cyan
Get-ChildItem -Path $ROOT -Recurse -Directory | Where-Object { $_.Name -like "$OLD*" } |
    Sort-Object { $_.FullName.Length } -Descending |
    ForEach-Object {
        try { Rename-Item $_.FullName -NewName ($_.Name -replace $OLD, $NEW) -ErrorAction Stop }
        catch { Write-Warning "Could not rename $($_.Name): $_" }
    }

# Bước 5: Đổi tên thư mục gốc
Write-Host "`n[5/5] Renaming root folder..." -ForegroundColor Cyan
Rename-Item -Path $ROOT -NewName ((Split-Path $ROOT -Leaf) -replace $OLD, $NEW)

Write-Host "`nDone! Project renamed from '$OLD' to '$NEW'." -ForegroundColor Green
```

**Cách dùng:**
```powershell
.\rename-project.ps1 -OLD "WorkHub" -NEW "DevHub" -ROOT "d:\WorkHub\BackEnd\WorkHub"
```

---

## Sau Khi Đổi Tên – Kiểm Tra

```powershell
# 1. Không còn chuỗi cũ trong file nào
$remaining = Get-ChildItem -Path "d:\WorkHub\BackEnd\WorkHub" -Recurse -Include "*.cs","*.csproj","*.slnx" |
    Select-String -Pattern "DevFlow" | Select-Object FileName, Line
if ($remaining) { $remaining | Format-Table } else { Write-Host "No DevFlow references found. OK." }

# 2. Không còn thư mục nào tên cũ
Get-ChildItem -Path "d:\WorkHub\BackEnd\WorkHub" -Recurse -Directory | Where-Object { $_.Name -like "*DevFlow*" }

# 3. Build thử
cd "d:\WorkHub\BackEnd\WorkHub"
dotnet build WorkHub.slnx
```

---

## Các Vấn Đề Thường Gặp

| Lỗi | Nguyên nhân | Giải pháp |
|-----|-------------|-----------|
| `Access is denied` khi rename folder | VS Code / OmniSharp đang lock DLL trong `bin/obj` | Xóa `bin/obj` trước (Bước 1), hoặc đóng VS Code |
| `Access is denied` cho thư mục gốc | VS Code workspace đang mở folder đó | Đóng VS Code hoàn toàn, rename bằng PowerShell ngoài |
| Build lỗi sau rename | File `.slnx` hoặc `.csproj` còn path cũ | Kiểm tra lại Bước 2, chạy script verification |
| Namespace mismatch | File `.cs` chưa được cập nhật | Tìm kiếm: `Select-String -Pattern "OldName" -Recurse` |
| Migration không hoạt động | `DbContext` class name đổi nhưng không re-generate | Chạy `dotnet ef migrations add RenameProject` cho mỗi module |
