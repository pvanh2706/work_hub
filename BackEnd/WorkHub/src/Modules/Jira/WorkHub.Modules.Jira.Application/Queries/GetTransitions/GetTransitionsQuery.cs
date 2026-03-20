using WorkHub.Modules.Jira.Domain.ValueObjects;
using WorkHub.Shared;
using MediatR;

namespace WorkHub.Modules.Jira.Application.Queries.GetTransitions;

public record GetTransitionsQuery(string JiraIssueKey) : IRequest<Result<IReadOnlyList<JiraTransition>>>;
