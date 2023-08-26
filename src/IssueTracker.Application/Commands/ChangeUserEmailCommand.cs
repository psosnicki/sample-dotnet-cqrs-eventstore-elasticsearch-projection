using FluentValidation;
using IssueTracker.Domain.Repositories;
using IssueTracker.Domain.User;
using MediatR;

namespace IssueTracker.Application.Commands;

public record ChangeUserEmailCommand(Guid UserId, string Email) : ICommand<Unit>;
public class ChangeUserEmailCommandHandler : IRequestHandler<ChangeUserEmailCommand, Unit>
{
    private readonly IEventStoreRepository _eventStoreRepository;

    public ChangeUserEmailCommandHandler(IEventStoreRepository eventStoreRepository)
    {
        _eventStoreRepository = eventStoreRepository;
    }

    public async Task<Unit> Handle(ChangeUserEmailCommand command, CancellationToken cancellationToken)
    {
        var currentUser = await _eventStoreRepository.LoadAsync<User>(command.UserId) ?? throw new Exception("user not found");
        currentUser.ChangeEmail(command.Email);
        await _eventStoreRepository.SaveAsync(currentUser);
        return Unit.Value;
    }
}

public class ChangeUserEmailCommandValidator : AbstractValidator<ChangeUserEmailCommand>
{
    public ChangeUserEmailCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Email).EmailAddress()
                           .NotEmpty();
    }
}