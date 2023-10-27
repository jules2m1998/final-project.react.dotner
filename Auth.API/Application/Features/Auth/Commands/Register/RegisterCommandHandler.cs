using Api.Common.Middlewares.Exceptions;
using Auth.API.Application.Contracts.Persistence;
using Auth.API.Application.Extensions;
using Auth.API.Domain;
using AutoMapper;
using MediatR;

namespace Auth.API.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterCommandResponse>
{
    private readonly ILogger<RegisterCommandHandler> _logger;
    private readonly IAuthRepository _authRepo;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(ILogger<RegisterCommandHandler> logger, IAuthRepository authRepo, IMapper mapper)
    {
        _logger = logger;
        _authRepo = authRepo;
        _mapper = mapper;
    }

    public async Task<RegisterCommandResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var validator = new RegisterCommandValidator();
        var validationResult = validator.Validate(request);
        if (validationResult is not null && validationResult.Errors.Count > 0)
        {
            throw new BadRequestException("Some data are not valid")
            {
                Errors = validationResult.ConvertToDictionnary()
            };
        }
        var isExist = await _authRepo.IsAlreadyExistAsync(request.Email, cancellationToken);

        if (isExist) return new RegisterCommandResponse
        {
            Message = "This user already exist",
            IsSuccess = false
        };

        var user = _mapper.Map<ApplicationUser>(request);
        var saveErrors = await _authRepo.RegisterUserAsync(user, request.Password, cancellationToken);
        if(saveErrors is not null && saveErrors.Count > 0) throw new BadRequestException("Some data are not valid")
        {
            Errors = saveErrors
        };

        var dto = _mapper.Map<RegisterDto>(user);

        return new RegisterCommandResponse
        {
            IsSuccess = true,
            Message = "User successfully registered",
            Result = dto
        };
    }
}
