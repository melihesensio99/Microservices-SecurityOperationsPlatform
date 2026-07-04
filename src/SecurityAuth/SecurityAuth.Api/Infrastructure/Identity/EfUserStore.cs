using Microsoft.EntityFrameworkCore;
using SecurityAuth.Api.Infrastructure.Persistence;

namespace SecurityAuth.Api.Infrastructure.Identity;

public sealed class EfUserStore : IUserStore
{
    private readonly AuthDbContext _dbContext;

    public EfUserStore(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> TryAddAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Users.AnyAsync(
            x => x.Username == user.Username || x.Email == user.Email,
            cancellationToken);

        if (exists)
        {
            return false;
        }

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return _dbContext.Users.SingleOrDefaultAsync(x => x.Username == username, cancellationToken);
    }

    public Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbContext.Users.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
    }
}
