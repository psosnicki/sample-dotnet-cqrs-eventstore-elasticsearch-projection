namespace IssueTracker.Application.Requests
{
    public record ChangeUserEmailRequest(Guid Id, string Email);
    public record CreateUserRequest(string Email, string Firstname, string Lastname);
    public record CreateIssueRequest(string Title, string Description, Guid? AssigneId, Guid? ReporterId);
    public record AssignIssueRequest(Guid IssueId, Guid AssigneId);
    public record CloseIssueRequest(Guid IssueId);
}
