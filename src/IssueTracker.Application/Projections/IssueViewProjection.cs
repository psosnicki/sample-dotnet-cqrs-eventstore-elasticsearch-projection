using IssueTracker.Application.Interfaces;
using IssueTracker.Application.Views;
using IssueTracker.Domain.Issue.Events;

namespace IssueTracker.Application.Projections
{
    public class IssueViewProjection : Projection<IssueView>
    {
        private readonly IViewRepository<UserView> _userRepository;

        public IssueViewProjection(IViewRepository<IssueView> viewRepository, IViewRepository<UserView> userRepository) : base(viewRepository)
        {
            _userRepository = userRepository;

            Project<IssueCreatedEvent>(OnIssueCreated, e => e.Id);
            Project<IssueAssignedEvent>(OnIssueAssigned, e => e.Id);
            Project<IssueClosedEvent>(OnIssueClosed, e => e.Id);
        }

        public async ValueTask<IssueView> OnIssueCreated(IssueCreatedEvent @event, IssueView view, CancellationToken cancellationToken)
        {        
            UserView? assignee = null;
            UserView? reporter = null;

            if (@event.AssigneId is not null)
                assignee = await _userRepository.Get(@event.AssigneId.Value, cancellationToken);
            if (@event.ReporterId is not null)
                reporter = await _userRepository.Get(@event.ReporterId.Value, cancellationToken);

            return view with
            {
                Id = @event.Id,
                Assigne = assignee?.Fullname ?? "",
                Reporter = reporter?.Fullname ?? "",
                Closed = false,
                AssignedId = @event.AssigneId,
                ReporterId = @event.ReporterId,
                CreatedBySystem = !@event.ReporterId.HasValue,
                Description = @event.Description,
                Title =  @event.Title
            };
        }

        public async ValueTask<IssueView> OnIssueAssigned(IssueAssignedEvent @event, IssueView view, CancellationToken cancellationToken)
        {
            var assignee = await _userRepository.Get(@event.AssigneId, cancellationToken);
            return view with
            {
                Assigne = assignee.Fullname,
                AssignedId = @event.AssigneId
            };
        }

        public async ValueTask<IssueView> OnIssueClosed(IssueClosedEvent @event, IssueView view, CancellationToken cancellationToken)
        {
            return view with
            {
                Closed = true
            };
        }
    }
}