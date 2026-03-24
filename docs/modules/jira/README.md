# Module Jira - Tăng Tốc Làm Việc Với Jira

Module này giúp team tương tác với Jira nhanh hơn, giảm thời gian thao tác thủ công từ 80-90%.

## 🎯 Tổng Quan Nhanh

**Vấn đề:** Tạo issue trên Jira mất 3-5 phút, phải điền nhiều field, quên log work  
**Giải pháp:** Quick create với templates, auto-fill, auto-log → chỉ còn 30 giây

## 📚 Nội Dung

### 1. [Tổng Quan](01-overview.md)
- Vấn đề cần giải quyết
- Giải pháp module cung cấp
- So sánh Before/After
- Key benefits

### 2. Tính Năng Chi Tiết
- [Tạo Issue Nhanh](02-features/quick-issue-creation.md)
- [Issue Templates](02-features/issue-templates.md)
- [Tự Động Log Work](02-features/auto-log-work.md)
- [Đồng Bộ Với Jira](02-features/sync-jira.md)
- [Thao Tác Hàng Loạt](02-features/bulk-operations.md)

### 3. Hướng Dẫn Theo Vai Trò
- [Dành cho Developers](03-user-guides/for-developers.md)
- [Dành cho Project Managers](03-user-guides/for-project-managers.md)
- [Dành cho QA/Testers](03-user-guides/for-qa-testers.md)
- [Dành cho Support Team](03-user-guides/for-support-team.md)

### 4. Tài Liệu Kỹ Thuật
- [Kiến Trúc Module](04-technical/architecture.md)
- [API Endpoints](04-technical/api-endpoints.md)
- [Database Schema](04-technical/database-schema.md)
- [Domain Models](04-technical/domain-models.md)
- [Tích Hợp Jira API](04-technical/jira-api-integration.md)

### 5. Configuration
- [Kết Nối Jira Account](05-configuration/jira-connection.md)
- [Tạo & Quản Lý Templates](05-configuration/template-setup.md)
- [Map Workflow](05-configuration/workflow-mapping.md)

### 6. Ví Dụ Thực Tế
- [Tạo Bug Report](06-examples/create-bug-report.md)
- [Tạo Feature Request](06-examples/create-feature-request.md)
- [Automation Scenarios](06-examples/automation-scenarios.md)

## 🚀 Bắt Đầu Nhanh

**1. Kết nối Jira:**
```
Settings → Integrations → Jira → Connect Account
```

**2. Tạo template đầu tiên:**
```
Jira → Templates → New Template → Bug Template
```

**3. Tạo issue đầu tiên:**
```
Ctrl+J → Nhập title → Enter
```

Done! Issue được tạo trên Jira với đầy đủ fields.

## 💡 Use Cases

- **Developer:** Tạo task nhanh khi coding, auto-log work khi commit
- **PM:** Quản lý templates, ưu tiên công việc
- **QA:** Report bug với screenshot, tìm issues tương tự
- **Support:** Escalate customer issues lên dev team

## 📊 Metrics

- **Tiết kiệm thời gian:** 80-90% (từ 3-5 phút → 30 giây)
- **Giảm lỗi:** 70% (nhờ templates và validation)
- **Tăng log work:** 3x (nhờ tự động hóa)

## 🔗 Related Modules

- [Knowledge Module](../knowledge/README.md) - Link issues với knowledge entries
- [Tasks Module](../tasks/README.md) - Sync tasks với Jira issues

## 🆘 Support

- [Troubleshooting](../../technical/troubleshooting.md#jira-module)
- [FAQ](03-user-guides/faq.md)
- [GitHub Issues](https://github.com/your-org/workhub/issues?q=is%3Aissue+label%3Ajira)

---

**Last Updated:** 2026-03-22  
**Status:** ✅ Phase 1 Complete
