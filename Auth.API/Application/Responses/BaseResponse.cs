namespace Auth.API.Application.Responses;

public class BaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Result { get; set; }
}
