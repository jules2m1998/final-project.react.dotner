using FluentValidation;

namespace Auth.API.Application.Features.Auth.Commands.AddUserToRole;

public class AddUserToRoleCommandValidator : AbstractValidator<AddUserToRoleCommand>
{
    public AddUserToRoleCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Role).NotEmpty();
    }
}
