using IssueTracker.Domain.Common;
using IssueTracker.Domain.User.Events;

namespace IssueTracker.Domain.User
{
    public class User : Aggregate
    {
        public string Email { get; private set; }
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }

        private User(Guid id,string email, string firstname, string lastname)
        {
            Id = id;
            Email = email;
            Firstname = firstname;
            Lastname = lastname;
            EnqueueDomainEvent(new UserCreatedEvent(Id,Email, Firstname, Lastname));
        }

        public static User CreateNewUser(Guid id, string email, string firstname, string lastname)
            =>new(id,email, firstname, lastname);

        public void ChangeEmail(string email)
        {
            Email = email;
            EnqueueDomainEvent(new UserEmailUpdatedEvent(Id, email));
        }

        public override void When(object @event)
        {
            switch (@event)
            {
                case UserCreatedEvent e: 
                    OnCreated(e);
                    break;
                case UserEmailUpdatedEvent e:
                    OnEmailUpdated(e);
                    break;
            }
        }

        private void OnEmailUpdated(UserEmailUpdatedEvent @event)
        {
            Id = @event.UserId;
            Email = @event.Email;
        }

        private void OnCreated(UserCreatedEvent @event)
        {
            Id = @event.UserId;
            Email = @event.Email;
            Firstname = @event.Firstname;
            Lastname = @event.Lastname;
        }

        public User() { }
    }
}
