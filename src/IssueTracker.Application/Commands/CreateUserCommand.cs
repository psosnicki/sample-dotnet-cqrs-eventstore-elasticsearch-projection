using FluentValidation;
using IssueTracker.Domain.Repositories;
using IssueTracker.Domain.User;
using MediatR;

namespace IssueTracker.Application.Commands;

public record CreateUserCommand(Guid Id, string Email, string Firstname, string Lastname) : ICommand<Guid>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand,Guid>
{
    private readonly IEventStoreRepository _eventStoreRepository;

    public CreateUserCommandHandler(IEventStoreRepository eventStoreRepository)
    {
        _eventStoreRepository = eventStoreRepository;
    }

    public async Task<Guid> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = User.CreateNewUser(command.Id, command.Email, command.Firstname, command.Lastname);
        await _eventStoreRepository.SaveAsync(user);
        return user.Id;
    }
}
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Firstname).NotEmpty();

        RuleFor(x => x.Lastname).NotEmpty();

        RuleFor(x => x.Email).EmailAddress()
                           .NotEmpty();
    }
}

