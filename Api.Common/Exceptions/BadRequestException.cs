namespace Api.Common.Middlewares.Exceptions;

public class BadRequestException : BaseException
{
    public BadRequestException(string? message = null) : base(message)
    {
    }
}
