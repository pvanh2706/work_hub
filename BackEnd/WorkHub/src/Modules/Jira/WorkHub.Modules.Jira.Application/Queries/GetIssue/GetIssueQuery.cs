using WorkHub.Modules.Jira.Domain.ValueObjects;
using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Jira.Application.Queries.GetIssue;

public record GetIssueQuery(string JiraIssueKey) : IRequest<Result<JiraIssueDetail>>;
