using Auth.API.Application.Features.Auth.Queries.Login;
using Auth.API.Integration.Tests.Contexts.Login;
using System.Net;
using System.Net.Http.Json;
using TechTalk.SpecFlow.Assist;

namespace Auth.API.Integration.Tests.Implementations;

[Binding]
public class Steps_Then
{
    private readonly LoginContext _loginContext;
    public Steps_Then(LoginContext loginContext)
    {
        _loginContext = loginContext;
    }

    [Then(@"I get a status (.*) response")]
    public void ThenIGetAStatusErrorResponse(int statusCode)
    {
        var response = _loginContext.Response;
        Assert.NotNull(response);
        Assert.Equal((HttpStatusCode)statusCode, response?.StatusCode);
    }

    [Then(@"I have following data as response data")]
    public async void ThenIHaveFollowingDataAsResponseData(Table table)
    {
        var expectedResponse = table.CreateInstance<LoginQueryResponse>();
        var response = await _loginContext.Response!.Content.ReadFromJsonAsync<LoginQueryResponse>();
        _loginContext.LoginQueryResponse = response;
        Assert.NotNull(response);
        Assert.Equal(expectedResponse.IsSuccess, response?.IsSuccess);
        Assert.Equal(expectedResponse.Message, response?.Message);
    }

    [Then(@"I have following result")]
    public void ThenIHaveFollowingResult(Table table)
    {
        var expectedDto = table.CreateInstance<LoginQueryDto>();
        var dto = _loginContext.LoginQueryResponse?.Result;

        Assert.NotNull(_loginContext.LoginQueryResponse);
        Assert.NotNull(dto);
        Assert.Equal(expectedDto.Email, dto.Email);
        Assert.Equal(expectedDto.Name, dto.Name);
        Assert.Equal(expectedDto.PhoneNumber, dto.PhoneNumber);
        Assert.Equal(expectedDto.Roles.First(), dto.Roles.First());
        Assert.Equal(expectedDto.Roles.Length, dto.Roles.Length);
    }
}
