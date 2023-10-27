using Api.Common.Middlewares.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Common.Unit.Tests.Middlewares;

public class CustomException : BaseException
{
    public CustomException(string? message) : base(message)
    {
    }
}
