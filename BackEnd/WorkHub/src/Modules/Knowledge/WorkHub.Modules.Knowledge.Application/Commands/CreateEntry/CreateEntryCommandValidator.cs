using FluentValidation;

namespace WorkHub.Modules.Knowledge.Application.Commands.CreateEntry;

public class CreateEntryCommandValidator : AbstractValidator<CreateEntryCommand>
{
    public CreateEntryCommandValidator()
    {
        RuleFor(x => x.SoftwareName)
            .NotEmpty().WithMessage("Software name is required.")
            .MaximumLength(100);

        RuleFor(x => x.ModuleName)
            .NotEmpty().WithMessage("Module name is required.")
            .MaximumLength(100);

        RuleFor(x => x.IssueTitle)
            .NotEmpty().WithMessage("Issue title is required.")
            .MaximumLength(500);

        RuleFor(x => x.RootCause)
            .NotEmpty().WithMessage("Root cause is required.");

        RuleFor(x => x.Fix)
            .NotEmpty().WithMessage("Fix description is required.");

        RuleFor(x => x.JiraIssueKey)
            .Matches(@"^[A-Z]+-\d+$").WithMessage("Jira issue key format must be like 'PMS-1234'.")
            .When(x => !string.IsNullOrEmpty(x.JiraIssueKey));
    }
}
