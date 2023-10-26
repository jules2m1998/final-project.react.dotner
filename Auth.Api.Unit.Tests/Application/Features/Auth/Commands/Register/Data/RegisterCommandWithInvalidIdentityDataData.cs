using Auth.API.Application.Exceptions;
using Auth.API.Application.Features.Auth.Commands.Register;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Api.Unit.Tests.Application.Features.Auth.Commands.Register.Data;

internal class RegisterCommandWithInvalidIdentityDataData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new RegisterCommand("mail@mail.com", "name", "99999999", "pass") { },
            new BadRequestException("Some data are not valid")
            {
                Errors = new Dictionary<string, string[]>()
                {
                    { nameof(RegisterCommand.Password), new string[] { "Password mut have at least 8 charaters" } },
                }
            }
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
