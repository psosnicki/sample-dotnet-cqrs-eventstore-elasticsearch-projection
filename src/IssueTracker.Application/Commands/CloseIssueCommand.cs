using IssueTracker.Domain.Repositories;
using IssueTracker.Domain.Issue;
using MediatR;

namespace IssueTracker.Application.Commands;

public record CloseIssueCommand(Guid IssueId) : ICommand<Unit>;

public class CloseIssueCommandHandler : IRequestHandler<CloseIssueCommand,Unit>
{
    private readonly IEventStoreRepository _eventStoreRepository;

    public CloseIssueCommandHandler(IEventStoreRepository eventStoreRepository)
    {
        _eventStoreRepository = eventStoreRepository;
    }

    public async Task<Unit> Handle(CloseIssueCommand command, CancellationToken cancellationToken)
    {
        var issue = await _eventStoreRepository.LoadAsync<Issue>(command.IssueId) ?? throw new Exception("issue not found");
        issue.Close();
        await _eventStoreRepository.SaveAsync(issue);
        return Unit.Value;
    }
}
