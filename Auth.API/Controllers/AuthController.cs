using Auth.API.Application.Contracts.Infrastructure.ContextAccessor;
using Auth.API.Application.Features.Auth.Commands.AddUserToRole;
using Auth.API.Application.Features.Auth.Commands.Register;
using Auth.API.Application.Features.Auth.Queries.Login;
using Auth.API.Application.Features.Auth.Queries.WhoAmI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IContextAccessor _contextAccessor;

    public AuthController(ISender sender, IContextAccessor contextAccessor)
    {
        _sender = sender;
        _contextAccessor = contextAccessor;
    }

    [HttpPost]
    [Route(nameof(Login))]
    public async Task<IActionResult> Login([FromBody] LoginQuery query)
    {
        var result = await _sender.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [Route(nameof(Register))]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpPost]
    [Route(nameof(AddUserToRole))]
    public async Task<IActionResult> AddUserToRole([FromBody] AddUserToRoleCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    [Route(nameof(WhoAmI))]
    public async Task<IActionResult> WhoAmI()
    {
        var query = new WhoAmIQuery
        {
            UserName = _contextAccessor.UserName ?? string.Empty,
        };
        var result = await _sender.Send(query, new CancellationToken());

        return Ok(result);
    }
}
