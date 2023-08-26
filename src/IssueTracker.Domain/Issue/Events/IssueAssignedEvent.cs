namespace IssueTracker.Domain.Issue.Events
{
    public record IssueAssignedEvent(Guid Id, Guid AssigneId);
}
