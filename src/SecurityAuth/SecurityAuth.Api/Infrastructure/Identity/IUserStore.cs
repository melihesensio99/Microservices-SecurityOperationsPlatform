namespace SecurityAuth.Api.Infrastructure.Identity;

public interface IUserStore
{
    Task<bool> TryAddAsync(ApplicationUser user, CancellationToken cancellationToken);
    Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
