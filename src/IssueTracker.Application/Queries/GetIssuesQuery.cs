using IssueTracker.Application.Interfaces;
using IssueTracker.Application.Views;
using MediatR;

namespace IssueTracker.Application.Queries
{
    public record GetIssuesQuery : IQuery<IEnumerable<IssueView>>;

    public class GetIssuesQueryHandler : IRequestHandler<GetIssuesQuery, IEnumerable<IssueView>>
    {
        private readonly IViewRepository<IssueView> _viewRepository;

        public GetIssuesQueryHandler(IViewRepository<IssueView> viewRepository)
        {
            _viewRepository = viewRepository;
        }

        public async Task<IEnumerable<IssueView>> Handle(GetIssuesQuery request, CancellationToken cancellationToken)
            => await _viewRepository.GetAll(cancellationToken);
    }
}
