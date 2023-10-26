namespace Auth.API.Application.Contracts.Infrastructure.ContextAccessor;

public interface IContextAccessor
{
    string? UserName { get; }
}
