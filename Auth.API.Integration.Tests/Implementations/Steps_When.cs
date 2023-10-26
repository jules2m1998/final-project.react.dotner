using Auth.API.Integration.Tests.Contexts.Login;
using Auth.API.Integration.Tests.ProgramConfiguration;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;

namespace Auth.API.Integration.Tests.Implementations;

[Binding]
public class Steps_When : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly LoginContext _loginContext;
    private readonly HttpClient _client;

    public Steps_When(CustomWebApplicationFactory<Program> factory, LoginContext loginContext)
    {
        _factory = factory;
        _loginContext = loginContext;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true
        });
    }

    [When(@"I launch an http login request with my informations")]
    public async Task WhenILaunchAnHttpLoginRequestWithMyInformations()
    {
        using var jsonContent = new StringContent(JsonSerializer.Serialize(_loginContext.LoginQuery), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/Auth/Login", jsonContent);
        _loginContext.Response = response;
    }
}
