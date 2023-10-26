using Auth.API.Application.Contracts.Persistence;
using Auth.API.Application.Exceptions;
using Auth.API.Application.Features.Auth.Queries.WhoAmI;
using Auth.API.Contants;
using Auth.API.Domain;
using AutoMapper;
using Moq;

namespace Auth.Api.Unit.Tests.Application.Features.Auth.Queries.WhoAmI;

public class WhoAmIQueryHandlerTests
{
    private readonly WhoAmIQueryHandler _handler;
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IAuthRepository> _authRepositoryMock = new();

    public WhoAmIQueryHandlerTests()
    {
        _handler = new WhoAmIQueryHandler(_authRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithIvalidUserNameThrowBadRequestException()
    {
        // Arrange
        var query = new WhoAmIQuery
        {
            UserName = ""
        };
        var expectedError = new Dictionary<string, string[]>
        {
            { "UserName", new string[] { "'User Name' must not be empty." } }
        };


        // Act
        var result = await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(query, CancellationToken.None));


        // Assert
        _authRepositoryMock.Verify(x => x.GetUserByUserNameAsync(query.UserName, CancellationToken.None), Times.Never);
        _authRepositoryMock.Verify(x => x.GetUserRolesAsync(It.IsAny<ApplicationUser>(), CancellationToken.None), Times.Never);
        _mapperMock.Verify(x => x.Map<WhoAmIQueryDto>(It.IsAny<ApplicationUser>()), Times.Never);

        Assert.NotNull(result);
        Assert.IsType<BadRequestException>(result);
        Assert.Equal("The user name is invalid", result.Message);
        Assert.Equal(expectedError, result.Errors);
    }

    [Fact]
    public async Task Handle_WithUnexistedUserNameReturnErrorResponseWithNuData()
    {
        // Arrange
        var query = new WhoAmIQuery
        {
            UserName = "Test"
        };
        ApplicationUser? user = null;

        _authRepositoryMock
            .Setup(x => x.GetUserByUserNameAsync(query.UserName, CancellationToken.None))
            .ReturnsAsync(user);


        // Act
        var result = await _handler.Handle(query, CancellationToken.None);


        // Assert
        _authRepositoryMock.Verify(x => x.GetUserByUserNameAsync(query.UserName, CancellationToken.None), Times.Once);
        _authRepositoryMock.Verify(x => x.GetUserRolesAsync(It.IsAny<ApplicationUser>(), CancellationToken.None), Times.Never);
        _mapperMock.Verify(x => x.Map<WhoAmIQueryDto>(It.IsAny<ApplicationUser>()), Times.Never);

        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal("This user doesn't exist.", result.Message);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task Handle_WithExistingUserName_ReturnSuccessResponseWithData() 
    {
        // Arrange
        var query = new WhoAmIQuery
        {
            UserName = "Test"
        };
        ApplicationUser user = new ApplicationUser
        {
            UserName = "Test"
        };
        var dto = new WhoAmIQueryDto
        {
            Id = Guid.Empty,
            UserName = user.UserName,
            Email = "mail",
            Name = "Name",
            PhoneNumber = "1234567890",
        };

        _authRepositoryMock
            .Setup(x => x.GetUserByUserNameAsync(query.UserName, CancellationToken.None))
            .ReturnsAsync(user);
        _authRepositoryMock
            .Setup(x => x.GetUserRolesAsync(user, CancellationToken.None))
            .ReturnsAsync(new string[]
            {
                Roles.MANAGER
            });
        _mapperMock.Setup(x => x.Map<WhoAmIQueryDto>(user)).Returns(dto);


        // Act
        var result = await _handler.Handle(query, CancellationToken.None);


        // Assert
        _authRepositoryMock.Verify(x => x.GetUserByUserNameAsync(query.UserName, CancellationToken.None), Times.Once);
        _authRepositoryMock.Verify(x => x.GetUserRolesAsync(It.IsAny<ApplicationUser>(), CancellationToken.None), Times.Once);
        _mapperMock.Verify(x => x.Map<WhoAmIQueryDto>(It.IsAny<ApplicationUser>()), Times.Once);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("User successfully found.", result.Message);
        Assert.NotNull(result.Result);

        dto.Roles = new string[]
        {
            Roles.MANAGER
        };

        Assert.Equivalent(dto, result.Result);
    }
}
