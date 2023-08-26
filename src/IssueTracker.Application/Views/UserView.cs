namespace IssueTracker.Application.Views;

public record UserView
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public string Fullname { get; init; }
    public required string Firstname { get; init; }
    public required string Lastname { get; init; }
}
