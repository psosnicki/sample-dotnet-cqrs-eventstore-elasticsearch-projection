namespace IssueTracker.Domain.Issue.Events
{
    public record IssueCreatedEvent(Guid Id,string Title, string Description, Guid? AssigneId = null, Guid? ReporterId = null);
}
