using Auth.API.Application.Contracts.Infrastructure.ContextAccessor;
using Auth.API.Infrastructure.ContextAccessors;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Auth.Api.Unit.Tests.Infrastructure.ContextAccessors;

public class ContextAccessorTests
{
    private readonly ContextAccessor _contextAccessor;
    private readonly Mock<IHttpContextAccessor> _contextAccessorMock = new();

    public ContextAccessorTests()
    {
        _contextAccessor = new(_contextAccessorMock.Object);
    }

    [Theory]
    [InlineData((string)null)]
    [InlineData("user 1")]
    [InlineData("user 2")]
    [InlineData("user 3")]
    public void UserName_MatchtToConnectedUserName(string username)
    {
        // Arrange
        _contextAccessorMock.Setup(x => x.HttpContext.User.Identity.Name).Returns(username);

        // Act
        var result = _contextAccessor.UserName;

        // Assert
        _contextAccessorMock.Verify(x => x.HttpContext.User.Identity.Name, Times.Once);
        Assert.Equal(username, result);
    }
}
