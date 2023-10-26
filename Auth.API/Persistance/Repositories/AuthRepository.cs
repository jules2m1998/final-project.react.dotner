using Auth.API.Application.Contracts.Persistence;
using Auth.API.Domain;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Persistance.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string[]?> AddUserToRoleAsync(string username, string role, CancellationToken token)
    {
        var user = await _userManager.FindByEmailAsync(username);
        if (user == null) return new string[] { "This user doesn't exist" };
        var result = await _userManager.AddToRoleAsync(user, role);
        if (result.Succeeded) return null;
        return result.Errors.Select(x => x.Description).ToArray();
    }

    public async Task<ApplicationUser?> GetUserByUserNameAndPasswordAsync(string username, string password, CancellationToken token)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return null;
        var passwordValid = await _userManager.CheckPasswordAsync(user, password);
        if(passwordValid) return user;
        return null;
    }

    public async Task<ApplicationUser?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        return await _userManager.FindByNameAsync(userName);

    }

    public async Task<string[]?> GetUserRolesAsync(ApplicationUser user, CancellationToken token)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToArray();
    }

    public async Task<bool> IsAlreadyExistAsync(string email, CancellationToken token)
    {
        var user = await _userManager.FindByNameAsync(email);
        return user is not null;
    }

    public async Task<Dictionary<string, string[]>?> RegisterUserAsync(ApplicationUser register, string password, CancellationToken token)
    {
        register.UserName = register.Email;
        var result = await _userManager.CreateAsync(register, password);
        if (!result.Succeeded)
        {
            var errors = from error in result.Errors
                          group error by error.Code into grouped
                          select grouped;
            return errors.ToDictionary(x => x.Key, x => x.Select(x => x.Description).ToArray());
        }
        return null;
    }
}
