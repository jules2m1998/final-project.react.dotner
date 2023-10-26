using MediatR;

namespace Auth.API.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Name, string PhoneNumber, string Password): IRequest<RegisterCommandResponse>;
