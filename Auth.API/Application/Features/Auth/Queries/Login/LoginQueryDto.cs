namespace Auth.API.Application.Features.Auth.Queries.Login;

public class LoginQueryDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string[] Roles {  get; set; } = Array.Empty<string>();
    public string Token {  get; set; } = string.Empty;
}
