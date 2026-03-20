using FluentValidation;

namespace WorkHub.Modules.Jira.Application.Commands.EditIssue;

public class EditIssueCommandValidator : AbstractValidator<EditIssueCommand>
{
    public EditIssueCommandValidator()
    {
        RuleFor(x => x.JiraIssueKey)
            .NotEmpty()
            .Matches(@"^[A-Z]+-\d+$").WithMessage("Jira issue key không hợp lệ, ví dụ: PMS-123.");

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty);

        // Ít nhất 1 field phải được cập nhật
        RuleFor(x => x)
            .Must(x => x.Summary != null
                       || x.Description != null
                       || x.PriorityId != null
                       || x.AssigneeAccountId != null
                       || (x.LabelsToAdd?.Count > 0)
                       || (x.LabelsToRemove?.Count > 0))
            .WithMessage("Phải có ít nhất một field cần cập nhật.");

        RuleFor(x => x.Summary)
            .MaximumLength(255)
            .When(x => x.Summary != null);
    }
}
