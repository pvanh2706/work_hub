# Jira Module - Tổng Quan

## Vấn Đề Cần Giải Quyết

### 1. Tạo Issue Trên Jira Tốn Thời Gian

**Hiện tại:**
- Mở browser → Login Jira
- Navigate đến project
- Click "Create Issue"
- Fill 10-15 fields manually:
  - Issue Type
  - Summary
  - Description
  - Priority
  - Assignee
  - Sprint
  - Components
  - Labels
  - Story Points
  - Epic Link
  - ...
- Click "Create"
- Copy link share với team

**Thời gian:** 3-5 phút/issue × 10+ issues/ngày = **30-50 phút/ngày**

### 2. Thiếu Chuẩn Hóa

- Mỗi người viết issue theo kiểu riêng
- Thiếu thông tin quan trọng
- Khó search và filter
- Quality không đồng đều

### 3. Quên Log Work

- Developers quên log work time
- PM khó track progress
- Reports không chính xác

### 4. Context Switching

- Đang code → phải chuyển sang browser
- Mất focus
- Giảm productivity

## Giải Pháp

### 1. Quick Issue Creation

**Workflow mới:**
```
Ctrl+J → Nhập title → Enter
```

Hệ thống tự động:
- Detect issue type từ keywords
- Apply template phù hợp
- Fill default values
- Create trên Jira
- Return link

**Thời gian:** 30 giây

### 2. Smart Templates

**Issue Templates có sẵn:**
- 🐛 Bug Report
- ✨ Feature Request
- 🔥 Hotfix
- 🎫 Support Ticket
- ♻️ Refactor
- 📝 Technical Debt

**Mỗi template bao gồm:**
- Pre-filled fields
- Structured description format
- Validation rules
- Auto-assignment rules

**Ví dụ Bug Template:**
```markdown
## Bug Description
[Auto-filled from title]

## Steps to Reproduce
1. 
2. 
3. 

## Expected Behavior
[Empty for user to fill]

## Actual Behavior
[Empty for user to fill]

## Environment
- Version: [Auto-detected]
- Browser: [Auto-detected]
- OS: [Auto-detected]

## Screenshots
[Drag & drop area]

## Additional Context
[Optional]
```

### 3. Auto Log Work

**Tích hợp với Git:**
```bash
git commit -m "feat(auth): implement login #PROJ-123"
# Tự động log 30 minutes work vào PROJ-123
```

**Tích hợp với Timer:**
- Start work session → auto track time
- Stop session → auto log to Jira

### 4. Stay trong App

- Không cần rời VS Code hoặc app
- Quick shortcuts (Ctrl+J, Ctrl+Shift+J)
- Inline preview của issues

## So Sánh Before/After

| Thao Tác | Before (Jira Web) | After (WorkHub) | Tiết Kiệm |
|----------|------------------|----------------|-----------|
| **Tạo bug report** | 3-5 phút | 30 giây | **83-90%** |
| **Tạo feature** | 4-6 phút | 45 giây | **81-88%** |
| **Log work** | 1-2 phút (thường quên) | 0 giây (tự động) | **100%** |
| **Update status** | 30 giây × 5 times/day | 0 giây (auto-sync) | **100%** |
| **Search issues** | 2-3 phút | 10 giây (integrated search) | **83-90%** |

**Tổng tiết kiệm trung bình:** **30-45 phút/ngày/người**

Với team 10 người: **5-7.5 giờ/ngày** = **25-37.5 giờ/tuần**

## Key Benefits

### Cho Developers
✅ Tạo issues nhanh không rời code  
✅ Auto-log work time  
✅ Keyboard shortcuts  
✅ Templates giảm thinking overhead  

### Cho Project Managers
✅ Chuẩn hóa issue quality  
✅ Accurate time tracking  
✅ Better insights (vì đầy đủ data)  
✅ Manage templates centralized  

### Cho QA/Testers
✅ Bug template với screenshots  
✅ Link đến test cases  
✅ Find similar bugs (AI-powered)  
✅ Quick reproduce steps template  

### Cho Support Team
✅ Escalate customer issues dễ dàng  
✅ Link tickets với Jira issues  
✅ Track resolution status  
✅ Knowledge base integration  

## Technical Highlights

- **Jira REST API integration:** Full CRUD operations
- **Webhook support:** Real-time sync Jira → WorkHub
- **Offline mode:** Queue actions khi mất mạng
- **Bulk operations:** Xử lý nhiều issues cùng lúc
- **Custom fields:** Support mọi custom fields của Jira

## Architecture Overview

```
┌─────────────┐         ┌──────────────┐         ┌────────────┐
│   User      │────────▶│  WorkHub API │────────▶│ Jira Cloud │
│  Interface  │         │  Jira Module │         │   REST API │
└─────────────┘         └──────────────┘         └────────────┘
      │                         │
      │                         ▼
      │                 ┌──────────────┐
      │                 │   Database   │
      │                 │  (Templates, │
      │                 │    Mapping)  │
      │                 └──────────────┘
      │
      ▼
┌─────────────┐
│  Real-time  │
│   Updates   │
│  (SignalR)  │
└─────────────┘
```

## What's Next?

- [Quick Issue Creation](02-features/quick-issue-creation.md) - Chi tiết feature
- [Setup Guide](05-configuration/jira-connection.md) - Kết nối Jira
- [Templates](02-features/issue-templates.md) - Tạo templates
- [Examples](06-examples/create-bug-report.md) - Ví dụ thực tế

---

**Last Updated:** 2026-03-22
