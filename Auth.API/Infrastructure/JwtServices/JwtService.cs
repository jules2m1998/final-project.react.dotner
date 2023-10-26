using Auth.API.Application.Contracts.Infrastructure.JwtService;
using Auth.API.Domain;
using Auth.API.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.API.Infrastructure.JwtServices;

public class JwtService : IJwtService
{
    private readonly JwtSettingModel _options;

    public JwtService(IOptions<JwtSettingModel> options)
    {
        _options = options.Value;
    }

    public string GenerateToken(ApplicationUser user, string[] roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_options.Secret);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserName),
            new Claim(ClaimTypes.Name, user.UserName),
        };
        var roleClaims = from role in roles
                        select new Claim(ClaimTypes.Role, role);

        claims.AddRange(roleClaims);

        var tokenDescription = new SecurityTokenDescriptor
        {
            Audience = _options.Audience,
            Issuer = _options.Issuer,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescription);
        return tokenHandler.WriteToken(token);
    }
}
