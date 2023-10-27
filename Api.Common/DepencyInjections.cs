using Api.Common.Contracts;
using Api.Common.Helpers;
using Api.Common.Mapper;
using Api.Common.Middlewares;
using Api.Common.Middlewares.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Common;

public static class DepencyInjections
{
    public static IServiceCollection AddCommonServices(this IServiceCollection @this)
    {
        @this.AddTransient<IHttpExceptionMapper>(x =>
        {
            var mapper = new HttpExceptionMapper();
            mapper.Map<BadRequestException>(StatusCodes.Status400BadRequest);
            mapper.Map<NotFoundException>(StatusCodes.Status404NotFound);
            return mapper;
        });
        @this.AddTransient<IResponseWritterHelper, ResponseWritterHelper>();

        return @this;
    }
    public static IApplicationBuilder CommonConfigureApp(this IApplicationBuilder @this)
    {
        @this.UseMiddleware<HandleExceptionMiddleware>();
        return @this;
    }
}
