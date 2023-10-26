using Auth.API.Contants;
using Auth.API.Domain;
using Auth.API.Persistance.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Auth.Api.Unit.Tests.Persistense.Repositories;

public class AuthRepositoryTests
{
    private readonly AuthRepository _authRepository;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock = new(new Mock<IUserStore<ApplicationUser>>().Object,
    new Mock<IOptions<IdentityOptions>>().Object,
    new Mock<IPasswordHasher<ApplicationUser>>().Object,
    Array.Empty<IUserValidator<ApplicationUser>>(),
    Array.Empty<IPasswordValidator<ApplicationUser>>(),
    new Mock<ILookupNormalizer>().Object,
    new Mock<IdentityErrorDescriber>().Object,
    new Mock<IServiceProvider>().Object,
    new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

    public AuthRepositoryTests()
    {
        _authRepository = new AuthRepository(_userManagerMock.Object);
    }

    [Fact]
    public async Task RegisterUserAsync_WithInvalidData_ReturnDictionnary()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserName = "Test",
            Name = "Test",
            Email = "Test",
            PhoneNumber = "Test",
        };
        var identityErrors = new IdentityError[]
        {
            new IdentityError()
            {
                Code = "Username already exists",
                Description = "Username already exists"
            }
        };
        _userManagerMock.Setup(x => x.CreateAsync(user, "complexPassw@rd123")).ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _authRepository.RegisterUserAsync(user, "complexPassw@rd123", CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.CreateAsync(user, "complexPassw@rd123"), Times.Once);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task RegisterUserAsync_WithValidData_ReturnNull()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserName = "Test",
            Name = "Test",
            Email = "Test",
            PhoneNumber = "Test",
        };
        _userManagerMock.Setup(x => x.CreateAsync(user, "complexPassw@rd123")).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authRepository.RegisterUserAsync(user, "complexPassw@rd123", CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.CreateAsync(user, "complexPassw@rd123"), Times.Once);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task IsAlreadyExistAsync_ReturnExpectedResultFromUserManager(bool given)
    {
        // Arrange
        var username = "test";
        ApplicationUser? user = given ? new ApplicationUser() : null;
        _userManagerMock.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);

        // Act
        var result = await _authRepository.IsAlreadyExistAsync(username, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.FindByNameAsync(username), Times.Once);

        Assert.Equal(given, result);
    }

    [Theory]
    [InlineData(Roles.CLIENT)]
    [InlineData(Roles.MANAGER)]
    public async Task GetUserRolesAsync_ReturnExpectedRoleFromUserManager(string role)
    {
        // Arrange
        var user = new ApplicationUser();
        var roles = new string[] { role };
        _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

        // Act
        var result = await _authRepository.GetUserRolesAsync(user, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.GetRolesAsync(user), Times.Once);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equivalent(roles, result);
    }

    [Theory]
    [InlineData("Error")]
    [InlineData(null)]
    public async Task AddUserToRoleAsync_ReturnTrueWhenErrorOccur(string? error)
    {
        // Arrange
        var user = new ApplicationUser();
        IdentityResult identityResult = 
            error is not null ? 
            IdentityResult.Failed(new IdentityError[] { new IdentityError { Code = error, Description = error } }) : 
            IdentityResult.Success;

        _userManagerMock.Setup(x => x.FindByEmailAsync("user")).ReturnsAsync(user);
        _userManagerMock
            .Setup(x => x.AddToRoleAsync(user, Roles.CLIENT))
            .ReturnsAsync(identityResult);

        // Act
        var result = await _authRepository.AddUserToRoleAsync("user", Roles.CLIENT, CancellationToken.None);
        // Assert
        _userManagerMock.Verify(x => x.FindByEmailAsync("user"), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(user, Roles.CLIENT), Times.Once);

        if(error == null)
        {
            Assert.Null(result);
        }
        else
        {
            Assert.NotNull(result);
            Assert.Equivalent(new string[] { error }, result);
        }
    }

    [Fact]
    public async Task AddUserToRoleAsync_WithUnexistUserReturnErrors()
    {
        // Arrange
        ApplicationUser? user = null;
        _userManagerMock.Setup(x => x.FindByEmailAsync("user")).ReturnsAsync(user);

        // Act
        var result = await _authRepository.AddUserToRoleAsync("user", Roles.CLIENT, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.FindByEmailAsync("user"), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetUserByUserNameAndPasswordAsync_ReturnNullWhenUserManagerReturnNotFoundUser()
    {
        // Arrange
        var username = "user";
        ApplicationUser? user = null;
        _userManagerMock.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);

        // Act
        var result = await _authRepository.GetUserByUserNameAndPasswordAsync(username, "pwd", CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.FindByNameAsync(username), Times.Once);
        _userManagerMock.Verify(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByUserNameAndPasswordAsync_ReturnNullWhenUserManagerReturnUserButPasswordInvalid()
    {
        // Arrange
        var username = "user";
        ApplicationUser user = new()
        {
            UserName = username
        };
        _userManagerMock.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "pwd")).ReturnsAsync(false);

        // Act
        var result = await _authRepository.GetUserByUserNameAndPasswordAsync(username, "pwd", CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.FindByNameAsync(username), Times.Once);
        _userManagerMock.Verify(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByUserNameAndPasswordAsync_ReturnDataWhenUserManagerFoundUserAndPasswordIsValid()
    {
        // Arrange
        var username = "user";
        ApplicationUser user = new()
        {
            UserName = username
        };
        _userManagerMock.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "pwd")).ReturnsAsync(true);

        // Act
        var result = await _authRepository.GetUserByUserNameAndPasswordAsync(username, "pwd", CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.FindByNameAsync(username), Times.Once);
        _userManagerMock.Verify(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetUserByUserNameAsync_WithUnexistingUser_ReturnNull()
    {
        // Arrange
        var username = "user";
        ApplicationUser? user = null;
        _userManagerMock.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);

        // Act
        var result = await _authRepository.GetUserByUserNameAsync(username, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.FindByNameAsync(username), Times.Once);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByUserNameAsync_WithExistingUser_ReturnNull()
    {
        // Arrange
        var username = "user";
        ApplicationUser user = new()
        {
            UserName = "user"
        };
        _userManagerMock.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);

        // Act
        var result = await _authRepository.GetUserByUserNameAsync(username, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.FindByNameAsync(username), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(username, result.UserName);
    }
}
