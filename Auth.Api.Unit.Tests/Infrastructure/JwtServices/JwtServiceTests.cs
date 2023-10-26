using Auth.API.Contants;
using Auth.API.Domain;
using Auth.API.Infrastructure.JwtServices;
using Auth.API.Models;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Api.Unit.Tests.Infrastructure.JwtServices;

public class JwtServiceTests
{
    private readonly JwtSettingModel _setting = new JwtSettingModel()
    {
        Secret = "KEYzdfsd f df dg fgdsf rfsdf sdfs dfs dfsf sf sdfsdfsdf sedfsedf s",
        Issuer = "ISSUER",
        Audience = "Audience"
    };

    private readonly Mock<IOptions<JwtSettingModel>> _options = new();
    private readonly JwtService _service;

    public JwtServiceTests()
    {
        _options.SetupGet(x => x.Value).Returns(_setting);
        _service = new JwtService(_options.Object);
    }

    [Fact]
    public void Generate_WithUserAndRolesReturnToken()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserName = "Test",
            Email = "Test",
            Id = Guid.Empty.ToString()
        };
        var roles = new List<string> { Roles.MANAGER };

        // Act
        var token = _service.GenerateToken(user, roles.ToArray());

        // Assert
        Assert.NotNull(token);
    }
}
