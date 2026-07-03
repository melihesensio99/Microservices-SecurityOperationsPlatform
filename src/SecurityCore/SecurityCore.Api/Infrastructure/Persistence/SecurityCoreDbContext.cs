using SecurityCore.Api.Domain.Incidents;

namespace SecurityCore.Api.Infrastructure.Persistence;

public sealed class SecurityCoreDbContext : DbContext
{
    public SecurityCoreDbContext(DbContextOptions<SecurityCoreDbContext> options)
        : base(options)
    {
    }

    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<IncidentNote> IncidentNotes => Set<IncidentNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var incident = modelBuilder.Entity<Incident>();
        var incidentNote = modelBuilder.Entity<IncidentNote>();

        incident.ToTable("Incidents");
        incident.HasKey(x => x.Id);
        incident.Property(x => x.Id).ValueGeneratedNever();
        incident.Property(x => x.Title).IsRequired().HasMaxLength(200);
        incident.Property(x => x.Description).IsRequired().HasMaxLength(2000);
        incident.Property(x => x.AssetName).HasMaxLength(200);
        incident.Property(x => x.CreatedBy).IsRequired().HasMaxLength(100);
        incident.Property(x => x.Status).IsRequired();
        incident.Property(x => x.Severity).IsRequired();
        incident.Property(x => x.CreatedAt).IsRequired();
        incident.Property(x => x.UpdatedAt).IsRequired();
        incident.HasMany(x => x.Notes)
            .WithOne()
            .HasForeignKey(x => x.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        incidentNote.ToTable("IncidentNotes");
        incidentNote.HasKey(x => x.Id);
        incidentNote.Property(x => x.Id).ValueGeneratedNever();
        incidentNote.Property(x => x.IncidentId).IsRequired();
        incidentNote.Property(x => x.Author).IsRequired().HasMaxLength(100);
        incidentNote.Property(x => x.Message).IsRequired().HasMaxLength(2000);
        incidentNote.Property(x => x.CreatedAt).IsRequired();
        incidentNote.HasIndex(x => x.IncidentId);

        base.OnModelCreating(modelBuilder);
    }
}
