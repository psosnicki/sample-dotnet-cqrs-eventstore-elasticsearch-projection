namespace IssueTracker.Domain.User.Events;

public record UserCreatedEvent(Guid UserId, string Email, string Firstname, string Lastname);
