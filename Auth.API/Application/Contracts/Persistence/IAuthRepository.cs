using Auth.API.Application.Features.Auth.Commands.Register;
using Auth.API.Domain;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Application.Contracts.Persistence;

public interface IAuthRepository
{
    public Task<Dictionary<string, string[]>?> RegisterUserAsync(ApplicationUser register, string password, CancellationToken token);
    public Task<bool> IsAlreadyExistAsync(string email, CancellationToken token);
    public Task<string[]?> AddUserToRoleAsync(string username, string role, CancellationToken token);
    public Task<ApplicationUser?> GetUserByUserNameAndPasswordAsync(string username, string password, CancellationToken token);
    public Task<string[]?> GetUserRolesAsync(ApplicationUser user, CancellationToken token);
    public Task<ApplicationUser?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken);
}
