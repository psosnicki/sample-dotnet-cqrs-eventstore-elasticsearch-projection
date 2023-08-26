using MediatR;

namespace IssueTracker.Application.Commands
{
    public interface ICommand<out TResult> : IRequest<TResult> { }
}
