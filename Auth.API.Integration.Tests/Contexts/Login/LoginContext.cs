using Auth.API.Application.Features.Auth.Queries.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.API.Integration.Tests.Contexts.Login;

public class LoginContext
{
    public LoginQuery LoginQuery { get; set; } = new LoginQuery();
    public LoginQueryResponse? LoginQueryResponse { get; set; } = new();
    public HttpResponseMessage? Response { get; set; } = null;
}
