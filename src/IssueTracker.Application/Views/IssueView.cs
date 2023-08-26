namespace IssueTracker.Application.Views
{
    public record IssueView
    {
        public required Guid Id { get; init; }
        public required string Title { get; init; }
        public required string Description { get; init; }
        public string? Assigne { get; init; }
        public string? Reporter { get; init; }
        public bool Closed { get; init; }
        public Guid? ReporterId { get; init; }
        public Guid? AssignedId { get; init; }
        public bool CreatedBySystem { get; init; }
    };
}
