namespace Api.Common.Middlewares.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string? message) : base(message)
    {
    }
}
