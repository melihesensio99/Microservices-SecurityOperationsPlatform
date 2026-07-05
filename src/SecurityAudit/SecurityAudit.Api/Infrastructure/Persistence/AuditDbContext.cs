using Microsoft.EntityFrameworkCore;
using SecurityAudit.Api.Domain.AuditLogs;

namespace SecurityAudit.Api.Infrastructure.Persistence;

public sealed class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options)
        : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(builder =>
        {
            builder.ToTable("audit_logs");
            builder.HasKey(auditLog => auditLog.Id);

            builder.Property(auditLog => auditLog.ServiceName)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(auditLog => auditLog.Action)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(auditLog => auditLog.Level)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(auditLog => auditLog.Details)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(auditLog => auditLog.ResourceType)
                .HasMaxLength(128);

            builder.Property(auditLog => auditLog.ResourceId)
                .HasMaxLength(128);

            builder.Property(auditLog => auditLog.ActorId)
                .HasMaxLength(128);

            builder.Property(auditLog => auditLog.ActorName)
                .HasMaxLength(256);

            builder.Property(auditLog => auditLog.CorrelationId)
                .HasMaxLength(128);

            builder.Property(auditLog => auditLog.MetadataJson)
                .HasMaxLength(4000);

            builder.HasIndex(auditLog => auditLog.OccurredAt);
            builder.HasIndex(auditLog => auditLog.ServiceName);
            builder.HasIndex(auditLog => auditLog.Level);
        });
    }
}
