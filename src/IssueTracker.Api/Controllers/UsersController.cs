using IssueTracker.Application.Commands;
using IssueTracker.Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult>  CreateUser([FromBody]CreateUserRequest createUserRequest)
        {
            var userId = await _mediator.Send(new CreateUserCommand(Guid.NewGuid(),createUserRequest.Email, createUserRequest.Firstname, createUserRequest.Lastname));
            return CreatedAtAction(nameof(CreateUser), userId);
        }

        [HttpPut]
        [Route("email")]
        public async Task<IActionResult> ChangeUserEmail([FromBody] ChangeUserEmailRequest updateEmailRequest)
        {
            await _mediator.Send(new ChangeUserEmailCommand(updateEmailRequest.Id, updateEmailRequest.Email));
            return Ok();
        }
    }
}