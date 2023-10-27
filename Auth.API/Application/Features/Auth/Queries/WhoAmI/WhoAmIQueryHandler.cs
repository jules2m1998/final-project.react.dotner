using Api.Common.Middlewares.Exceptions;
using Auth.API.Application.Contracts.Persistence;
using AutoMapper;
using MediatR;

namespace Auth.API.Application.Features.Auth.Queries.WhoAmI;

public class WhoAmIQueryHandler : IRequestHandler<WhoAmIQuery, WhoAmIQueryResponse>
{
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;

    public WhoAmIQueryHandler(IAuthRepository authRepository, IMapper mapper)
    {
        _authRepository = authRepository;
        _mapper = mapper;
    }

    public async Task<WhoAmIQueryResponse> Handle(WhoAmIQuery request, CancellationToken cancellationToken)
    {
        var validator = new WhoAmIQueryValidator();
        var validatorResult = validator.Validate(request);
        if (!validatorResult.IsValid)
        {
            throw new BadRequestException("The user name is invalid")
            {
                Errors = validatorResult.ToDictionary().ToDictionary(x => x.Key, x => x.Value)
            };
        }
        var user = await _authRepository.GetUserByUserNameAsync(request.UserName, cancellationToken);
        if (user is null) return new WhoAmIQueryResponse
        {
            IsSuccess = false,
            Message = "This user doesn't exist.",
            Result = null
        };
        var dto = _mapper.Map<WhoAmIQueryDto>(user);
        dto.Roles = await _authRepository.GetUserRolesAsync(user, cancellationToken) ?? Array.Empty<string>();

        return new WhoAmIQueryResponse
        {
            IsSuccess = true,
            Message = "User successfully found.",
            Result = dto
        };
    }
}
