using MediatR;

namespace Auth.API.Application.Features.Auth.Queries.WhoAmI;

public class WhoAmIQuery : IRequest<WhoAmIQueryResponse>
{
    public string UserName { get; set; } = string.Empty;
}
