namespace Auth.API.Application.Exceptions;

public class NotFoundException : BaseException<string>
{
    public NotFoundException(string? message) : base(message)
    {
    }
}
