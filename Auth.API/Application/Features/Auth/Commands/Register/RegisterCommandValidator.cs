using FluentValidation;

namespace Auth.API.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(r => r.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(r => r.Name)
            .NotEmpty()
            .NotEmpty();

        RuleFor(r => r.Password)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.PhoneNumber)
            .Matches(@"^(?:\+\d{1,3}\s?)?[\d\s\-]{6,14}$")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("{PropertyName} is not a valid phone number");
    }
}
