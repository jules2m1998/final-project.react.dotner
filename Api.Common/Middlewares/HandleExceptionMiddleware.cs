using Api.Common.Contracts;
using Api.Common.Mapper;
using Api.Common.Middlewares.Exceptions;
using Api.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace Api.Common.Middlewares;

public class HandleExceptionMiddleware
{
    private readonly RequestDelegate next;
    private readonly IHttpExceptionMapper httpMapper;
    private readonly IResponseWritterHelper helper;

    public HandleExceptionMiddleware(RequestDelegate next, IHttpExceptionMapper httpMapper, IResponseWritterHelper helper)
    {
        this.next = next;
        this.httpMapper = httpMapper;
        this.helper = helper;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BaseException ex)
        {
            var mapper = httpMapper.Mapper;

            var error = new ErrorResponses
            {
                Title = "Internal server error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occured ont the server.",
                Details = null
            };
            if (mapper.TryGetValue(ex.GetType(), out var code))
                error = new ErrorResponses
                {
                    Title = ex.Message,
                    Status = code,
                    Detail = ex.Message,
                    Details = ex.Errors
                };

            context.Response.StatusCode = error.Status;
            context.Response.ContentType = "application/json";
            await helper.WriteAsync(context.Response, error);
        }
    }
}
