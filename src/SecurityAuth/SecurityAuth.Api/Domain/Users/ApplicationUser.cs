namespace SecurityAuth.Api.Domain.Users;

public sealed class ApplicationUser
{
    public ApplicationUser(Guid id, string username, string email)
    {
        Id = id;
        Username = username;
        Email = email;
    }

    public Guid Id { get; }
    public string Username { get; }
    public string Email { get; }
    public string? PasswordHash { get; private set; }
    public List<string> Roles { get; } = [];

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void AddRole(string role)
    {
        if (!Roles.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
            Roles.Add(role);
        }
    }
}
