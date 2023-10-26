using Auth.API.Application.Contracts.Persistence;
using Auth.API.Application.Exceptions;
using Auth.API.Application.Features.Auth.Commands.AddUserToRole;
using Moq;

namespace Auth.Api.Unit.Tests.Application.Features.Auth.Commands.AddUserToRole;

public class AddUserToRoleCommandHandlerTests
{
    private readonly AddUserToRoleCommandHandler _handler;
    private readonly Mock<IAuthRepository> _authRepositoryMock = new();

    public AddUserToRoleCommandHandlerTests()
    {
        _handler = new AddUserToRoleCommandHandler(_authRepositoryMock.Object);
    }
    // TODO : Test invalid fluent validator result
    [Fact]
    public async Task Handle_WithInValidCommandData_ThrowBadRequestException()
    {
        // Arrange
        var command = new AddUserToRoleCommand();
        var expectedErrors = new Dictionary<string, string[]>
        {
            { nameof(AddUserToRoleCommand.Username), new string[] { "'Username' must not be empty." } },
            { nameof(AddUserToRoleCommand.Role), new string[] { "'Role' must not be empty." } },
        };

        // Act
        var result = await Assert.ThrowsAsync<BadRequestException>(async () => await _handler.Handle(command, CancellationToken.None));

        // Assert
        _authRepositoryMock.Verify(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _authRepositoryMock.Verify(x => x.AddUserToRoleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.NotNull(result);
        Assert.IsType<BadRequestException>(result);
        Assert.Equal("Some fields are invalid.", result.Message);
        Assert.Equal(expectedErrors, result.Errors);

    }
    // TODO : Not exist user
    [Fact]
    public async Task Handle_WithNotExistUsername_ThrowNotFoundException()
    {
        // Arrange
        var command = new AddUserToRoleCommand
        {
            Username = "user",
            Role = "role"
        };
        _authRepositoryMock
            .Setup(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _authRepositoryMock.Verify(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _authRepositoryMock.Verify(x => x.AddUserToRoleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.NotNull(result);
        Assert.Equal("This user doesn't exist.", result.Message);
        Assert.False(result.IsSuccess);
        Assert.Null(result.Result);
    }
    // TODO : All right
    [Fact]
    public async Task Handle_WithCorrectCommandData_AddUserToRole()
    {
        // Arrange
        var command = new AddUserToRoleCommand
        {
            Username = "user",
            Role = "role"
        };
        string[]? errors = null;
        _authRepositoryMock
            .Setup(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _authRepositoryMock
            .Setup(x => x.AddUserToRoleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(errors);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _authRepositoryMock.Verify(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _authRepositoryMock.Verify(x => x.AddUserToRoleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.True(result.IsSuccess);
        Assert.Equal("User succesfully add to role.", result.Message);
    }
    // TODO : All right
    [Fact]
    public async Task Handle_WithCorrectCommandData_ReturnErrorResponse()
    {
        // Arrange
        var command = new AddUserToRoleCommand
        {
            Username = "user",
            Role = "role"
        };
        var errors = new string[] {"TEST"};
        _authRepositoryMock
            .Setup(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _authRepositoryMock
            .Setup(x => x.AddUserToRoleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(errors);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _authRepositoryMock.Verify(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _authRepositoryMock.Verify(x => x.AddUserToRoleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(result.IsSuccess);
        Assert.Equal("Something when wrong during registration", result.Message);
        Assert.Equivalent(result.Result, errors);
    }


}
