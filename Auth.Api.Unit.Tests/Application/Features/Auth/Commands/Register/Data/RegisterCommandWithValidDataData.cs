using Auth.API.Application.Features.Auth.Commands.Register;
using System.Collections;

namespace Auth.Api.Unit.Tests.Application.Features.Auth.Commands.Register.Data;

public class RegisterCommandWithValidDataData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new RegisterCommand("test@test.com", "Tester", "+23055555555", "Coplex-password@12345"),
            new RegisterCommandResponse
            {
                IsSuccess = true,
                Message = "User successfully registered",
                Result = new RegisterDto
                {
                    Id = Guid.Empty,
                    UserName = "test@test.com",
                    Email = "test@test.com",
                    Name = "Tester",
                    PhoneNumber = "+23055555555",
                }
            }
        };
        yield return new object[]
        {
            new RegisterCommand("test2@test.com", "Tester 2", "+23055555556", "Coplex-password@12346"),
            new RegisterCommandResponse
            {
                IsSuccess = true,
                Message = "User successfully registered",
                Result = new RegisterDto
                {
                    Id = Guid.Empty,
                    UserName = "test2@test.com",
                    Email = "test2@test.com",
                    Name = "Tester2",
                    PhoneNumber = "+23055555556",
                }
            }
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
