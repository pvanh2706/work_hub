using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Knowledge.Application.Commands.CreateEntry;

public record CreateEntryCommand(
    string SoftwareName,
    string ModuleName,
    string IssueTitle,
    string Description,
    string RootCause,
    string Fix,
    string? FixVersion,
    string? JiraIssueKey,
    List<string> Tags,
    Guid CreatedBy
) : IRequest<Result<Guid>>;
