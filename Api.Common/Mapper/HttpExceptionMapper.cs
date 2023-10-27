using Api.Common.Contracts;
using Api.Common.Middlewares.Exceptions;

namespace Api.Common.Mapper;

public class HttpExceptionMapper : IHttpExceptionMapper
{
    private readonly Dictionary<Type, int> _mapper = new();
    public Dictionary<Type, int> Mapper => _mapper;

    public HttpExceptionMapper Map<TException>(int statusCode) where TException : BaseException
    {
        _mapper.Add(typeof(TException), statusCode);
        return this;
    }
}
