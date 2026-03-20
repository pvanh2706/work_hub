using FluentValidation;

namespace WorkHub.Modules.Jira.Application.Commands.CreateIssue;

public class CreateIssueCommandValidator : AbstractValidator<CreateIssueCommand>
{
    public CreateIssueCommandValidator()
    {
        RuleFor(x => x.ProjectKey)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(@"^[A-Z][A-Z0-9]+$").WithMessage("Project key phải là chữ hoa, ví dụ: PMS, DEV.");

        RuleFor(x => x.Summary)
            .NotEmpty().WithMessage("Tiêu đề issue là bắt buộc.")
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Mô tả issue là bắt buộc.");

        RuleFor(x => x.IssueTypeId)
            .NotEmpty().WithMessage("Issue type ID là bắt buộc.");

        RuleFor(x => x.PriorityId)
            .NotEmpty().WithMessage("Priority ID là bắt buộc.");

        RuleFor(x => x.OrganizationId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty);
    }
}
