using MediatR;

namespace Auth.API.Application.Features.Auth.Queries.Login;

public class LoginQuery: IRequest<LoginQueryResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
