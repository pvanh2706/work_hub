using FluentValidation;

namespace WorkHub.Modules.Jira.Application.Commands.TransitionIssue;

public class TransitionIssueCommandValidator : AbstractValidator<TransitionIssueCommand>
{
    public TransitionIssueCommandValidator()
    {
        RuleFor(x => x.JiraIssueKey)
            .NotEmpty()
            .Matches(@"^[A-Z]+-\d+$").WithMessage("Jira issue key không hợp lệ, ví dụ: PMS-123.");

        RuleFor(x => x.TransitionId)
            .NotEmpty().WithMessage("Transition ID là bắt buộc. Gọi GET /transitions trước để lấy danh sách.");

        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty);
    }
}
