using Api.Common.Contracts;
using Api.Common.Responses;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Api.Common.Helpers;

public class ResponseWritterHelper : IResponseWritterHelper
{
    public async Task WriteAsync(HttpResponse response, ErrorResponses error)
    {
        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        await response.WriteAsync(JsonSerializer.Serialize(error, serializeOptions));
    }
}
