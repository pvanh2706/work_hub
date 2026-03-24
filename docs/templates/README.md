# Documentation Templates

Templates này giúp tạo documentation structure cho các sản phẩm khác trong công ty.

## Cách Sử Dụng

### 1. Copy Template Structure

```bash
# Copy toàn bộ structure
cp -r docs/templates/product-docs-structure/ ../YourProduct/docs/

# Hoặc cherry-pick từng phần
cp docs/templates/product-docs-structure/01-overview.template.md ../YourProduct/docs/overview.md
```

### 2. Replace Placeholders

Tìm và thay thế các placeholders:
- `{Product_Name}` → Tên sản phẩm của bạn
- `{Product_Type}` → Loại sản phẩm (Web app, Mobile app, API...)
- `{Target_Users}` → Người dùng mục tiêu
- `{Main_Problem}` → Vấn đề chính giải quyết
- `{Backend_Stack}` → Tech stack backend
- `{Frontend_Stack}` → Tech stack frontend
- ... (xem full list trong mỗi template)

### 3. Customize Content

- Xóa sections không cần thiết
- Thêm sections đặc thù cho sản phẩm
- Fill content theo guideline trong template

## Templates Có Sẵn

### A. Product Documentation Structure

Cho người đọc (developers, users, business):

```
product-docs-structure/
├── README.template.md
├── 01-overview.template.md
├── 02-getting-started.template.md
├── 03-user-guides.template/
│   ├── README.template.md
│   └── for-{role}.template.md
├── 04-technical.template/
│   ├── architecture.template.md
│   ├── api-docs.template.md
│   └── ...
├── 05-modules.template/
│   └── {module-name}/
│       ├── 01-overview.template.md
│       ├── 02-features.template/
│       ├── 03-user-guides.template/
│       ├── 04-technical.template/
│       ├── 05-configuration.template/
│       └── 06-examples.template/
└── 06-business.template/
```

### B. AI Context Structure

Cho AI assistants:

```
ai-context-structure/
├── product-overview.template.md
├── architecture-patterns.template.md
├── development-commands.template.md
└── business-domain.template.md
```

## Examples

Xem WorkHub documentation làm ví dụ:
- [WorkHub docs/](../) - Human-readable docs
- [WorkHub /memories/repo/](../../..) - AI context

## Best Practices

1. **Viết cho audience cụ thể:** Dev, QA, PM cần info khác nhau
2. **DRY:** Tránh duplicate, dùng links
3. **Keep updated:** Review docs mỗi sprint/release
4. **Examples matter:** Code examples thực tế > lý thuyết
5. **Search-friendly:** Dùng keywords, headers rõ ràng

## Customization

Nếu template không fit:
1. Fork và modify theo nhu cầu
2. Share improvements qua PR
3. Document why you deviated (trong product README)

## Support

Questions về templates:
- Check WorkHub docs làm reference
- Open issue với label `docs-templates`
- Slack: #documentation-guild
