using Auth.API.Application.Contracts.Infrastructure.JwtService;
using Auth.API.Application.Contracts.Persistence;
using Auth.API.Application.Exceptions;
using Auth.API.Application.Features.Auth.Queries.Login;
using Auth.API.Contants;
using Auth.API.Domain;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Api.Unit.Tests.Application.Features.Auth.Queries.Login;

public class LoginQueryHandlerTests
{
    private readonly LoginQueryHandler _handler;
    private readonly Mock<IAuthRepository> _authRepoMock = new();
    private readonly Mock<IJwtService> _jwtService = new();
    private readonly Mock<IMapper> _mapperMock = new();

    public LoginQueryHandlerTests()
    {
        _handler = new LoginQueryHandler(_authRepoMock.Object, _jwtService.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithInvalidLoginQuery_ThrowBadRequestWithErrorMessages()
    {
        // Arrange
        var query = new LoginQuery();
        var errors = new Dictionary<string, string[]>
        {
            {nameof(LoginQuery.Username), new string[] { "'Username' must not be empty." } },
            {nameof(LoginQuery.Password), new string[] { "'Password' must not be empty." } }
        };

        // Act
        var result = await Assert.ThrowsAsync<BadRequestException>(async () => await _handler.Handle(query, CancellationToken.None));

        // Assert
        _authRepoMock
            .Verify(x => x.GetUserByUserNameAndPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None), Times.Never);
        _authRepoMock
            .Verify(x => x.GetUserRolesAsync(It.IsAny<ApplicationUser>(), CancellationToken.None), Times.Never);
        _jwtService.Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<string[]>()), Times.Never);
        _mapperMock.Verify(x => x.Map<LoginQueryDto>(It.IsAny<ApplicationUser>()), Times.Never);


        Assert.NotNull(result);
        Assert.Equal("Some fields are invalid", result.Message);
        Assert.Equivalent(errors, result.Errors);
    }

    // Username or password invalid
    [Fact]
    public async Task Handle_WithIncorrectUserNameOrPassword_ReturnLoginQueryResponseWithErrorMessage()
    {
        // Arrange
        ApplicationUser? user = null;
        var query = new LoginQuery()
        {
            Username = "test",
            Password = "1234567"
        };
        _authRepoMock
            .Setup(x => x.GetUserByUserNameAndPasswordAsync(query.Username, query.Password, CancellationToken.None))
            .ReturnsAsync(user);
        var expected = new LoginQueryResponse
        {
            IsSuccess = false,
            Message = "Incorrect username or password"
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _authRepoMock
            .Verify(x => x.GetUserByUserNameAndPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None), Times.Once);
        _authRepoMock
            .Verify(x => x.GetUserRolesAsync(It.IsAny<ApplicationUser>(), CancellationToken.None), Times.Never);
        _jwtService
            .Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<string[]>()), Times.Never);
        _mapperMock
            .Verify(x => x.Map<LoginQueryDto>(It.IsAny<ApplicationUser>()), Times.Never);

        Assert.NotNull(result);
        Assert.Equivalent(expected, result);
    }

    // Username and password valid
    [Fact]
    public async Task Handle_WithCorrectUserNameAndPassword_ReturnLoginQueryResponseWithSuccessMessage()
    {

        // Arrange
        ApplicationUser? user = new()
        {
            UserName = "test",
        };
        var query = new LoginQuery()
        {
            Username = "test",
            Password = "1234567"
        };
        _authRepoMock
            .Setup(x => x.GetUserByUserNameAndPasswordAsync(query.Username, query.Password, CancellationToken.None))
            .ReturnsAsync(user);
        _authRepoMock
            .Setup(x => x.GetUserRolesAsync(user, CancellationToken.None))
            .ReturnsAsync(new string[] { Roles.MANAGER });
        _jwtService.Setup(x => x.GenerateToken(user, new string[] { Roles.MANAGER })).Returns("TOKEN");
        _mapperMock.Setup(x => x.Map<LoginQueryDto>(It.IsAny<ApplicationUser>())).Returns(new LoginQueryDto
        {
            UserName = "test",
        });
        var expected = new LoginQueryDto
        {
            UserName = "test",
            Token = "TOKEN",
            Roles = new[] { Roles.MANAGER }
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert

        _authRepoMock
            .Verify(x => x.GetUserByUserNameAndPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None), Times.Once);
        _authRepoMock
            .Verify(x => x.GetUserRolesAsync(It.IsAny<ApplicationUser>(), CancellationToken.None), Times.Once);
        _jwtService
            .Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<string[]>()), Times.Once);
        _mapperMock
            .Verify(x => x.Map<LoginQueryDto>(It.IsAny<ApplicationUser>()), Times.Once);

        Assert.NotNull(result);
        Assert.Equal("User logged in successfully", result.Message);
        Assert.True(result.IsSuccess);
        Assert.Equivalent(expected, result.Result);
    }

    // Username and password valid but user doesn't have any role
    [Fact]
    public async Task Handle_WithCorrectUserNameAndPassword_ReturnLoginQueryResponseWithErrorMessage()
    {
        // Arrange
        ApplicationUser? user = new()
        {
            UserName = "test",
        };
        var query = new LoginQuery()
        {
            Username = "test",
            Password = "1234567"
        };
        _authRepoMock
            .Setup(x => x.GetUserByUserNameAndPasswordAsync(query.Username, query.Password, CancellationToken.None))
            .ReturnsAsync(user);
        _authRepoMock
            .Setup(x => x.GetUserRolesAsync(user, CancellationToken.None))
            .ReturnsAsync(Array.Empty<string>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _authRepoMock
            .Verify(x => x.GetUserByUserNameAndPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None), Times.Once);
        _authRepoMock
            .Verify(x => x.GetUserRolesAsync(It.IsAny<ApplicationUser>(), CancellationToken.None), Times.Once);
        _jwtService
            .Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<string[]>()), Times.Never);
        _mapperMock
            .Verify(x => x.Map<LoginQueryDto>(It.IsAny<ApplicationUser>()), Times.Never);

        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal("You dont have any role.", result.Message);
        Assert.Null(result.Result);
    }
}
