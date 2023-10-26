using Microsoft.AspNetCore.Identity;

namespace Auth.API.Domain;

public class ApplicationUser: IdentityUser
{
    public string Name { get; set; } = string.Empty;
}
