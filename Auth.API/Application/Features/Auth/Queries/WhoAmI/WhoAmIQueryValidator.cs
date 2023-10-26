using FluentValidation;

namespace Auth.API.Application.Features.Auth.Queries.WhoAmI;

public class WhoAmIQueryValidator : AbstractValidator<WhoAmIQuery>
{
    public WhoAmIQueryValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
    }
}
