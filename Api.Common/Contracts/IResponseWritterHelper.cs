using Api.Common.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Common.Contracts;

public interface IResponseWritterHelper
{
    Task WriteAsync(Microsoft.AspNetCore.Http.HttpResponse response, ErrorResponses error);
}
