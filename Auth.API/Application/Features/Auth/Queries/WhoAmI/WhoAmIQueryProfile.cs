using Auth.API.Domain;
using AutoMapper;

namespace Auth.API.Application.Features.Auth.Queries.WhoAmI;

public class WhoAmIQueryProfile : Profile
{
    public WhoAmIQueryProfile()
    {
        CreateMap<ApplicationUser, WhoAmIQueryDto>();
    }
}
