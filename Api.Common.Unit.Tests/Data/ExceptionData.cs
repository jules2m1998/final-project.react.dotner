using Api.Common.Middlewares.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Api.Common.Unit.Tests.Data;

public class ExceptionData
{
    private static BadRequestException BadRequest => new("Some fields are invalid")
    {
        Errors = new Dictionary<string, string[]>
        {
            {"ErrorField", new string[] {"Error 1", "Error 2"} }
        }
    };

    private static NotFoundException NotFound => new("This data not found")
    {
        Errors = new Dictionary<string, string[]>
        {
            {"NotFoundField", new string[] {"Error 1", "Error 2"} }
        }
    };

    public static IEnumerable<object[]> Data => new List<object[]>
    {
        new object[] { BadRequest, StatusCodes.Status400BadRequest },
        new object[] { NotFound, StatusCodes.Status404NotFound }
    };
}
