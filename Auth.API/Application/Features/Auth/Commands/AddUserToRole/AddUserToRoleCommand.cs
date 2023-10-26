using MediatR;

namespace Auth.API.Application.Features.Auth.Commands.AddUserToRole;

public class AddUserToRoleCommand : IRequest<AddUserToRoleCommandResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
