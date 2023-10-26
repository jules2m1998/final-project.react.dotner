using Auth.API.Application.Contracts.Persistence;
using Auth.API.Application.Exceptions;
using Auth.API.Application.Extensions;
using MediatR;

namespace Auth.API.Application.Features.Auth.Commands.AddUserToRole;

public class AddUserToRoleCommandHandler : IRequestHandler<AddUserToRoleCommand, AddUserToRoleCommandResponse>
{
    private readonly IAuthRepository _authRepository;

    public AddUserToRoleCommandHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<AddUserToRoleCommandResponse> Handle(AddUserToRoleCommand request, CancellationToken cancellationToken)
    {
        var validator = new AddUserToRoleCommandValidator();
        var validationResult = validator.Validate(request);
        if (validationResult is not null && validationResult.Errors.Count > 0)
        {
            throw new BadRequestException("Some fields are invalid.")
            {
                Errors = validationResult.ConvertToDictionnary()
            };
        }
        var isUserExist = await _authRepository.IsAlreadyExistAsync(request.Username, cancellationToken);
        if (!isUserExist) return new AddUserToRoleCommandResponse()
        {
            IsSuccess = false,
            Message = "This user doesn't exist.",
            Result = null
        };

        var result = await _authRepository.AddUserToRoleAsync(request.Username, request.Role, cancellationToken);
        if (result is not null and not { Length: 0 }) return new AddUserToRoleCommandResponse()
        {
            IsSuccess = false,
            Message = "Something when wrong during registration",
            Result = result
        };
        return new AddUserToRoleCommandResponse()
        {
            IsSuccess = true,
            Message = "User succesfully add to role."
        };
    }
}
