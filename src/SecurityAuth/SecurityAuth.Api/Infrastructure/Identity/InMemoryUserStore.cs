using System.Collections.Concurrent;

namespace SecurityAuth.Api.Infrastructure.Identity;

public sealed class InMemoryUserStore : IUserStore
{
    private readonly ConcurrentDictionary<string, ApplicationUser> _usersByUsername = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, ApplicationUser> _usersByEmail = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _gate = new();

    public Task<bool> TryAddAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            if (_usersByUsername.ContainsKey(user.Username) || _usersByEmail.ContainsKey(user.Email))
            {
                return Task.FromResult(false);
            }

            _usersByUsername[user.Username] = user;
            _usersByEmail[user.Email] = user;
            return Task.FromResult(true);
        }
    }

    public Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        _usersByUsername.TryGetValue(username, out var user);
        return Task.FromResult(user);
    }

    public Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        _usersByEmail.TryGetValue(email, out var user);
        return Task.FromResult(user);
    }
}
