using Auth.Api.Unit.Tests.Application.Features.Auth.Commands.Register.Data;
using Auth.API.Application.Contracts.Persistence;
using Auth.API.Application.Exceptions;
using Auth.API.Application.Features.Auth.Commands.Register;
using Auth.API.Domain;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;

namespace Auth.Api.Unit.Tests.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandlerTests
{
    private readonly RegisterCommandHandler _handler;
    private readonly Mock<IAuthRepository> _authRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<RegisterCommandHandler>> _loggerMock = new();
    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(_loggerMock.Object, _authRepositoryMock.Object, _mapperMock.Object);
    }

    [Theory]
    [ClassData(typeof(RegisterCommandWithInvalidDataData))]
    public async Task Handle_WithInvalidData_ThrowCustomBadRequestExeceptionWithErrors(RegisterCommand command, BadRequestException exception)
    {
        // Act
        var result = await Assert.ThrowsAsync<BadRequestException>(async () => await _handler.Handle(command, CancellationToken.None));

        // Assert
        _mapperMock.Verify(x => x.Map<RegisterDto>(It.IsAny<ApplicationUser>()), Times.Never);
        _mapperMock.Verify(x => x.Map<ApplicationUser>(It.IsAny<RegisterCommand>()), Times.Never);
        _authRepositoryMock.Verify(x => x.RegisterUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _authRepositoryMock.Verify(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.NotNull(result);
        Assert.IsType<BadRequestException>(result);
        Assert.Equal(exception.Message, result.Message);
        Assert.Equivalent(exception.Errors, result.Errors);
    }



    [Theory]
    [ClassData(typeof(RegisterCommandWithInvalidIdentityDataData))]
    public async Task Handler_WithValidCommandButInvalidIdentity_ThrowCustomBadRequestExeceptionWithErrors(RegisterCommand command, BadRequestException exception)
    {
        // Arrange
        var registerReturn = new Dictionary<string, string[]>
        {
            { nameof(RegisterCommand.Password), new string[] { "Password mut have at least 8 charaters" } }
        };
        _authRepositoryMock
            .Setup(x => x.RegisterUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registerReturn);
        _authRepositoryMock
            .Setup(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await Assert.ThrowsAsync<BadRequestException>(async () => await _handler.Handle(command, CancellationToken.None));

        // Assert
        _mapperMock.Verify(x => x.Map<RegisterDto>(It.IsAny<ApplicationUser>()), Times.Never);
        _mapperMock.Verify(x => x.Map<ApplicationUser>(It.IsAny<RegisterCommand>()), Times.Once);
        _authRepositoryMock.Verify(x => x.RegisterUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _authRepositoryMock.Verify(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(result);
        Assert.IsType<BadRequestException>(result);
        Assert.Equal(exception.Message, result.Message);
        Assert.Equivalent(exception.Errors, result.Errors);
    }

    [Theory]
    [ClassData(typeof(RegisterCommandWithValidDataData))]
    public async Task Handle_WithValidData_Return_RegisterCommandResponseWithRegisterDtoAsResult(RegisterCommand command, RegisterCommandResponse response)
    {
        // Arrange
        var expectedUserDto = response.Result!;
        _mapperMock
            .Setup(x => x.Map<RegisterDto>(It.IsAny<ApplicationUser>()))
            .Returns(expectedUserDto);

        _mapperMock
            .Setup(x => x.Map<ApplicationUser>(It.IsAny<RegisterCommand>()))
            .Returns(new ApplicationUser
            {
                UserName = command.Email,
                Email = command.Email,
                Name = command.Name,
                PhoneNumber = command.PhoneNumber
            });

        Dictionary<string, string[]>? registerReturn = null;
        _authRepositoryMock
            .Setup(x => x.RegisterUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(registerReturn);

        _authRepositoryMock
            .Setup(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapperMock.Verify(x => x.Map<RegisterDto>(It.IsAny<ApplicationUser>()), Times.Once);
        _mapperMock.Verify(x => x.Map<ApplicationUser>(It.IsAny<RegisterCommand>()), Times.Once);
        _authRepositoryMock.Verify(x => x.RegisterUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _authRepositoryMock.Verify(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(result);
        Assert.IsType<RegisterCommandResponse>(result);
        Assert.Equivalent(response, result);
    }

    [Fact]
    public async Task Handler_WithExistingUser_Return_RegisterCommandResponseWithErrorMessageAndSuccessFalse()
    {
        // Arrange
        _authRepositoryMock
            .Setup(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new RegisterCommand("test@test.com", "Tester", "+23055555555", "Coplex-password@12345");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);


        // Assert
        _mapperMock.Verify(x => x.Map<RegisterDto>(It.IsAny<ApplicationUser>()), Times.Never);
        _mapperMock.Verify(x => x.Map<ApplicationUser>(It.IsAny<RegisterCommand>()), Times.Never);
        _authRepositoryMock.Verify(x => x.RegisterUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _authRepositoryMock.Verify(x => x.IsAlreadyExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(result);
        Assert.IsType<RegisterCommandResponse>(result);
        Assert.True(!result.IsSuccess);
        Assert.Equal("This user already exist", result.Message);
    }
}
