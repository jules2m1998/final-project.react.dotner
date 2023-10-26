using Auth.API.Application.Contracts.Infrastructure.ContextAccessor;

namespace Auth.API.Infrastructure.ContextAccessors;

public class ContextAccessor : IContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public string? UserName => _httpContextAccessor.HttpContext.User.Identity.Name;

    public ContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
}
