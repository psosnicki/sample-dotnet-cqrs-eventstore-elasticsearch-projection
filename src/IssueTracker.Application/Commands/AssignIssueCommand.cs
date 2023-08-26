using IssueTracker.Application.Exceptions;
using IssueTracker.Domain.Issue;
using IssueTracker.Domain.Repositories;
using IssueTracker.Domain.User;
using MediatR;

namespace IssueTracker.Application.Commands;

public record AssignIssueCommand(Guid IssueId, Guid AssigneId) : ICommand<Unit>;

public class AssignIssueCommandHandler : IRequestHandler<AssignIssueCommand,Unit>
{
    private readonly IEventStoreRepository _eventStoreRepository;

    public AssignIssueCommandHandler(IEventStoreRepository eventStoreRepository)
    {
        _eventStoreRepository = eventStoreRepository;
    }

    public async Task<Unit> Handle(AssignIssueCommand command, CancellationToken cancellationToken)
    {
        var issue = await _eventStoreRepository.LoadAsync<Issue>(command.IssueId) ?? throw new NotFoundException($"Unable to find issue {command.IssueId}!");
        var assigne = await _eventStoreRepository.LoadAsync <User>(command.AssigneId) ?? throw new NotFoundException($"User {command.AssigneId} doesnt exist!");
        issue.Assign(command.AssigneId);
        await _eventStoreRepository.SaveAsync(issue);
        return Unit.Value;
    }
}
