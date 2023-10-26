using Auth.API.Application.Contracts.Infrastructure.ContextAccessor;
using Auth.API.Application.Features.Auth.Commands.AddUserToRole;
using Auth.API.Application.Features.Auth.Commands.Register;
using Auth.API.Application.Features.Auth.Queries.Login;
using Auth.API.Application.Features.Auth.Queries.WhoAmI;
using Auth.API.Contants;
using Auth.API.Controllers;
using Auth.API.Migrations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Auth.Api.Unit.Tests.Controllers;

public class AuthControllerTests
{
    private readonly AuthController _authController;
    private readonly Mock<ISender> _senderMock = new();
    private readonly Mock<IContextAccessor> _contextAccessorMock = new();

    public AuthControllerTests()
    {
        _authController = new(_senderMock.Object, _contextAccessorMock.Object);
    }

    [Theory]
    [InlineData(true, "User connected")]
    [InlineData(false, "Error found")]
    public async Task Login_CallSendMethodFromMediatorWithParamsQuery(bool isSuccess, string message)
    {
        // Arrange
        var query = new LoginQuery
        {
            Username = "username",
            Password = "password",
        };
        var response = new LoginQueryResponse
        {
            IsSuccess = isSuccess,
            Message = message,
        };

        _senderMock.Setup(x => x.Send(query, CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _authController.Login(query);

        // Assert
        _senderMock.Verify(x => x.Send(query, CancellationToken.None), Times.Once);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var value = ((OkObjectResult)result).Value;
        Assert.IsType<LoginQueryResponse>(value);
        Assert.Equivalent(response, value);

    }

    [Theory]
    [InlineData(true, "User connected")]
    [InlineData(false, "Error found")]
    public async Task Register_CallSendMethodFromMediatorWithParamsQuery(bool isSuccess, string message)
    {
        // Arrange
        var command = new RegisterCommand("test@test.com", "test", "690981056", "12345678");
        var response = new RegisterCommandResponse
        {
            IsSuccess = isSuccess,
            Message = message,
        };

        _senderMock.Setup(x => x.Send(command, CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _authController.Register(command);

        // Assert
        _senderMock.Verify(x => x.Send(command, CancellationToken.None), Times.Once);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var value = ((OkObjectResult)result).Value;
        Assert.IsType<RegisterCommandResponse>(value);
        Assert.Equivalent(response, value);

    }


    [Theory]
    [InlineData(true, "User connected")]
    [InlineData(false, "Error found")]
    public async Task AddUserToRole_CallSendMethodFromMediatorWithParamsQuery(bool isSuccess, string message)
    {
        // Arrange
        var command = new AddUserToRoleCommand
        {
            Username = "test",
            Role = Roles.MANAGER
        };
        var response = new AddUserToRoleCommandResponse
        {
            IsSuccess = isSuccess,
            Message = message,
        };

        _senderMock.Setup(x => x.Send(command, CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _authController.AddUserToRole(command);

        // Assert
        _senderMock.Verify(x => x.Send(command, CancellationToken.None), Times.Once);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var value = ((OkObjectResult)result).Value;
        Assert.IsType<AddUserToRoleCommandResponse>(value);
        Assert.Equivalent(response, value);

    }

    [Theory]
    [InlineData(true, "User connected")]
    [InlineData(false, "Error found")]
    public async Task WhoAmI_CallSendMethodFromMediatorWithParamsQuery(bool isSuccess, string message)
    {
        // Arrange
        var response = new WhoAmIQueryResponse
        {
            IsSuccess = isSuccess,
            Message = message,
        };

        _senderMock.Setup(x => x.Send(It.IsAny<WhoAmIQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);
        _contextAccessorMock.SetupGet(x => x.UserName).Returns("test");

        // Act
        var result = await _authController.WhoAmI();

        // Assert
        _senderMock.Verify(x => x.Send(It.IsAny<WhoAmIQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        _contextAccessorMock.Verify(x => x.UserName, Times.Once);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var value = ((OkObjectResult)result).Value;
        Assert.IsType<WhoAmIQueryResponse>(value);
        Assert.Equivalent(response, value);
    }
}
