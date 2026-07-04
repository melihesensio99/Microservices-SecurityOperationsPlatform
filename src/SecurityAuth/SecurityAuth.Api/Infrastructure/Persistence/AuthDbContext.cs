using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace SecurityAuth.Api.Infrastructure.Persistence;

public sealed class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();

            entity.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.PasswordHash)
                .HasMaxLength(500);

            entity.Property(x => x.Roles)
                .HasColumnName("RolesJson")
                .HasConversion(
                    roles => JsonSerializer.Serialize(roles, (JsonSerializerOptions?)null),
                    json => string.IsNullOrWhiteSpace(json)
                        ? new List<string>()
                        : JsonSerializer.Deserialize<List<string>>(json, (JsonSerializerOptions?)null) ?? new List<string>());
        });
    }
}
