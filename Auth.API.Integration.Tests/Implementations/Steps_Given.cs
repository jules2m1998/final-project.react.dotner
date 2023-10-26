using Auth.API.Application.Features.Auth.Commands.Register;
using Auth.API.Application.Features.Auth.Queries.Login;
using Auth.API.Contants;
using Auth.API.Domain;
using Auth.API.Integration.Tests.Contexts.Login;
using Auth.API.Integration.Tests.ProgramConfiguration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Assist;

namespace Auth.API.Integration.Tests.Implementations;

[Binding]
public class Steps_Given : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly LoginContext _loginContext;

    public Steps_Given(CustomWebApplicationFactory<Program> factory, LoginContext loginContext)
    {
        _factory = factory;
        _loginContext = loginContext;
    }

    [Given(@"I have these following users in database")]
    public async Task GivenIHaveTheseFollowingUsersInDatabase(Table table)
    {
        using var scope = _factory.Services.CreateScope(); 

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var users = table.CreateSet<RegisterCommand>();
        foreach(var command in users)
        {
            var manager = new ApplicationUser
            {
                UserName = command.Email,
                Email = command.Email,
                Name = command.Name,
                PhoneNumber = command.PhoneNumber
            };
            var result = await userManager.CreateAsync(manager, command.Password);
            var resultAdmin = await userManager.AddToRoleAsync(manager, Roles.MANAGER);
        }
    }

    [Given(@"I have this information as connection information")]
    public void GivenIHaveThisInformationAsConnectionInformation(Table table)
    {
        _loginContext.LoginQuery = table.CreateInstance<LoginQuery>();
    }
}
