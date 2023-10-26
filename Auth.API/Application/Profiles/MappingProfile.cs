using Auth.API.Application.Features.Auth.Commands.Register;
using Auth.API.Application.Features.Auth.Queries.Login;
using Auth.API.Domain;
using AutoMapper;

namespace Auth.API.Application.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterCommand, ApplicationUser>();
        CreateMap<ApplicationUser, RegisterDto>();

        CreateMap<ApplicationUser, LoginQueryDto>();
    }
}
