namespace Auth.API.Application.Exceptions;

public class BadRequestException : BaseException<Dictionary<string, string[]>>
{
    public BadRequestException(string? message = null) : base(message)
    {
    }
}
