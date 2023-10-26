using Auth.API.Domain;

namespace Auth.API.Application.Contracts.Infrastructure.JwtService;

public interface IJwtService
{
    public string GenerateToken(ApplicationUser user, string[] roles);
}
