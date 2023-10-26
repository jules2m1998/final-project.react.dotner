namespace Auth.API.Application.Exceptions;

public abstract class BaseException<T> : Exception
{
    public BaseException(string? message) : base(message)
    {
    }

    public T? Errors { get; set; }
}
