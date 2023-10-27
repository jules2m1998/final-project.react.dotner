using Api.Common.Contracts;
using Api.Common.Mapper;
using Api.Common.Middlewares.Exceptions;
using Api.Common.Unit.Tests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Common.Unit.Tests.Mapper;

public class HttpExceptionMapperTests
{
    private readonly IHttpExceptionMapper _mapper = new HttpExceptionMapper();

    [Theory]
    [MemberData(nameof(ExceptionData.Data), MemberType = typeof(ExceptionData))]
    public void Map_WithExceptionAndStatus_AddExceptionTypeAndStatusCodeToMapCollection<T>(T _, int statusCode) where T : BaseException
    {
        // Arrange
        // Act
        var result = _mapper.Map<T>(statusCode);
        // Assert

        Assert.NotNull(result);
        Assert.NotNull(result.Mapper);
        Assert.Single(result.Mapper);
        var first = result.Mapper.First();
        Assert.Equal(typeof(T), first.Key);
        Assert.Equal(statusCode, first.Value);
    }
}
