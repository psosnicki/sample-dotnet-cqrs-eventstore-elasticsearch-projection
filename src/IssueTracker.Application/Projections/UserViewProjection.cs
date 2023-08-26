using IssueTracker.Application.Interfaces;
using IssueTracker.Application.Views;
using IssueTracker.Domain.User.Events;

namespace IssueTracker.Application.Projections
{
    public class UserViewProjection : Projection<UserView>
    {
        public UserViewProjection(IViewRepository<UserView> viewRepository) : base(viewRepository)
        {
            Project<UserCreatedEvent>(OnUserCreated, e => e.UserId);
            Project<UserEmailUpdatedEvent>(OnUserEmailUpdated, e => e.UserId);
        }

        public async ValueTask<UserView> OnUserEmailUpdated(UserEmailUpdatedEvent @event, UserView view, CancellationToken cancellationToken)
        {
            return view with
            {
                Email = @event.Email
            };
        }

        public async ValueTask<UserView> OnUserCreated(UserCreatedEvent @event, UserView view, CancellationToken cancellationToken)
        {
            return view with
            {
                Id = @event.UserId,
                Firstname = @event.Firstname,
                Lastname = @event.Lastname,
                Email = @event.Email,
                Fullname = $"{@event.Firstname} {@event.Lastname}"
            };
        }
    }
}