namespace Api.Common.Middlewares.Exceptions;

public abstract class BaseException : Exception
{
    public BaseException(string? message) : base(message)
    {
    }

    public Dictionary<string, string[]>? Errors { get; set; }
}
