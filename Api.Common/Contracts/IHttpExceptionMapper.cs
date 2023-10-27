using Api.Common.Mapper;
using Api.Common.Middlewares.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Common.Contracts;

public interface IHttpExceptionMapper
{
    Dictionary<Type, int> Mapper { get; }
    HttpExceptionMapper Map<TException>(int statusCode) where TException : BaseException;
}
