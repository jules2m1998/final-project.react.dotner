using Api.Common.Contracts;
using Api.Common.Mapper;
using Api.Common.Middlewares;
using Api.Common.Middlewares.Exceptions;
using Api.Common.Responses;
using Api.Common.Unit.Tests.Data;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Common.Unit.Tests.Middlewares;

public class HandleExceptionMiddlewareTests
{
    private readonly HandleExceptionMiddleware _middleWare;
    private readonly Mock<RequestDelegate> _nextMock = new();
    private readonly Mock<IHttpExceptionMapper> _httpMapperMock = new();
    private readonly Mock<HttpContext> _httpContextMock = new();
    private readonly Mock<IResponseWritterHelper> _helperMock = new();

    public HandleExceptionMiddlewareTests()
    {
        _middleWare = new(_nextMock.Object, _httpMapperMock.Object, _helperMock.Object);
        var dict = new Dictionary<Type, int>()
        {
            {typeof(BadRequestException), StatusCodes.Status400BadRequest },
            {typeof(NotFoundException), StatusCodes.Status404NotFound }
        };

        _httpMapperMock.SetupGet(x => x.Mapper).Returns(dict);
    }

    [Fact]
    public async Task InvokeAsync_WithNextNotThrowException_DoesnEditResponse()
    {
        // Arrange

        // Act
        await _middleWare.InvokeAsync(_httpContextMock.Object);

        // Assert
        _httpContextMock.VerifySet(x => x.Response.StatusCode = It.IsAny<int>(), Times.Never());
        _httpContextMock.VerifySet(x => x.Response.ContentType = It.IsAny<string>(), Times.Never());
        _helperMock.Verify(x => x.WriteAsync(It.IsAny<HttpResponse>(), It.IsAny<ErrorResponses>()), Times.Never());
    }
    // Not registered exception return 500 error

    [Fact]
    public async Task InvokeAsync_WithNextThrowExceptionNotRegisteredException_EditResponsToInternalServerError()
    {
        // Arrange
        var setSatusCode = StatusCodes.Status200OK;
        var setContentType = string.Empty;
        var exception = new CustomException("Exe");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);
        _httpContextMock.SetupSet(x => x.Response.StatusCode = It.IsAny<int>()).Callback<int>(x => setSatusCode = x);
        _httpContextMock.SetupSet(x => x.Response.ContentType = It.IsAny<string>()).Callback<string>(x => setContentType = x);

        // Act
        await _middleWare.InvokeAsync(_httpContextMock.Object);

        // Assert
        _httpContextMock.VerifySet(x => x.Response.StatusCode = It.IsAny<int>(), Times.Once());
        _httpContextMock.VerifySet(x => x.Response.ContentType = It.IsAny<string>(), Times.Once());
        _helperMock.Verify(x => x.WriteAsync(It.IsAny<HttpResponse>(), It.IsAny<ErrorResponses>()), Times.Once());
        Assert.Equal(StatusCodes.Status500InternalServerError, setSatusCode);
        Assert.Equal("application/json", setContentType);
    }
    // Registered exception edit response to registered status code and errorReponse data
    [Theory]
    [MemberData(nameof(ExceptionData.Data), MemberType = typeof(ExceptionData))]
    public async Task InvokeAsync_WithNextThrowExceptionRegisteredException_EditResponsToGivenStatusCodeAndErrorResponseBody(BaseException ex, int statusCode)
    {
        // Arrange
        var setSatusCode = StatusCodes.Status200OK;
        var setContentType = string.Empty;
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(ex);
        _httpContextMock.SetupSet(x => x.Response.StatusCode = It.IsAny<int>()).Callback<int>(x => setSatusCode = x);
        _httpContextMock.SetupSet(x => x.Response.ContentType = It.IsAny<string>()).Callback<string>(x => setContentType = x);

        // Act
        await _middleWare.InvokeAsync(_httpContextMock.Object);

        // Assert
        _httpContextMock.VerifySet(x => x.Response.StatusCode = It.IsAny<int>(), Times.Once());
        _httpContextMock.VerifySet(x => x.Response.ContentType = It.IsAny<string>(), Times.Once());
        _helperMock.Verify(x => x.WriteAsync(_httpContextMock.Object.Response, It.IsAny<ErrorResponses>()), Times.Once());
        Assert.Equal(statusCode, setSatusCode);
        Assert.Equal("application/json", setContentType);
    }
}
