namespace IssueTracker.Domain.User.Events
{
    public record UserEmailUpdatedEvent(Guid UserId, string Email);
}
