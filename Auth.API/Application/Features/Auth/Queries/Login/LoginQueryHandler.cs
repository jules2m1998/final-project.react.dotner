using Api.Common.Middlewares.Exceptions;
using Auth.API.Application.Contracts.Infrastructure.JwtService;
using Auth.API.Application.Contracts.Persistence;
using Auth.API.Application.Extensions;
using AutoMapper;
using MediatR;

namespace Auth.API.Application.Features.Auth.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginQueryResponse>
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public LoginQueryHandler(IAuthRepository authRepository, IJwtService jwtService, IMapper mapper)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<LoginQueryResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var validator = new LoginQueryValidator();
        var validatorResult = validator.Validate(request);

        if (!validatorResult.IsValid) throw new BadRequestException("Some fields are invalid")
        {
            Errors = validatorResult.ConvertToDictionnary()
        };
        var user = await _authRepository.GetUserByUserNameAndPasswordAsync(request.Username, request.Password, cancellationToken);
        if (user is null) return new LoginQueryResponse
        {
            IsSuccess = false,
            Message = "Incorrect username or password"
        };
        var roles = await _authRepository.GetUserRolesAsync(user, cancellationToken);
        if (roles is null or { Length : 0}) return new LoginQueryResponse
        {
            IsSuccess = false,
            Message = "You dont have any role."
        };
        var token = _jwtService.GenerateToken(user, roles);

        var dto = _mapper.Map<LoginQueryDto>(user);
        dto.Token = token;
        dto.Roles = roles;

        return new LoginQueryResponse
        {
            IsSuccess = true,
            Message = "User logged in successfully",
            Result = dto
        };
    }
}
