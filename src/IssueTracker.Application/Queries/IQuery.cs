using MediatR;

namespace IssueTracker.Application.Queries
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}
