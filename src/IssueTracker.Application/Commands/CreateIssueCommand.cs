using FluentValidation;
using IssueTracker.Domain.Issue;
using IssueTracker.Domain.Repositories;
using MediatR;

namespace IssueTracker.Application.Commands;

public record CreateIssueCommand(Guid IssueId, string Title, string Description, Guid? ReporterId, Guid? AssigneId = null) : ICommand<Guid>;

public class CreateIssueCommandHandler : IRequestHandler<CreateIssueCommand,Guid>
{
    private readonly IEventStoreRepository _eventStoreRepository;

    public CreateIssueCommandHandler(IEventStoreRepository eventStoreRepository)
    {
        _eventStoreRepository = eventStoreRepository;
    }

    public async Task<Guid> Handle(CreateIssueCommand command, CancellationToken cancellationToken)
    {
        var issue = Issue.CreateNewIssue(command.IssueId, command.Title, command.Description, command.AssigneId, command.ReporterId);
        await _eventStoreRepository.SaveAsync(issue);
        return issue.Id;
    }
}

public class CreateIssueCommandValidator : AbstractValidator<CreateIssueCommand>
{
    public CreateIssueCommandValidator()
    {
        RuleFor(x => x.IssueId).NotEmpty();
        RuleFor(x=>x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
    }
}
