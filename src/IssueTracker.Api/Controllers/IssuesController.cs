using IssueTracker.Application.Commands;
using IssueTracker.Application.Queries;
using IssueTracker.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Api.Controllers
{
    [Route("api/issues")]
    [ApiController]
    public class IssuesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public IssuesController(IMediator mediator,ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult>  CreateIssue([FromBody]CreateIssueRequest createIssueRequest)
        {
            var issueId = await _mediator.Send(new CreateIssueCommand(Guid.NewGuid(), createIssueRequest.Title, createIssueRequest.Description, createIssueRequest.ReporterId, createIssueRequest.AssigneId));
            return CreatedAtAction(nameof(CreateIssue), issueId);
        }

        [HttpPost]
        [Route("assign")]
        public async Task<IActionResult> AssignIssue([FromBody] AssignIssueRequest assignIssueRequest)
        {
            await _mediator.Send(new AssignIssueCommand(assignIssueRequest.IssueId, assignIssueRequest.AssigneId));
            return Ok();
        }

        [HttpPost]
        [Route("close")]
        public async Task<IActionResult> CloseIssue([FromBody] CloseIssueRequest closeIssueRequest)
        {
            await _mediator.Send(new CloseIssueCommand(closeIssueRequest.IssueId));
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllIssues()
        {
            return Ok(await _mediator.Send(new GetIssuesQuery()));
        }
    }
}