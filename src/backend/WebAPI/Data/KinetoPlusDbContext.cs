using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Data;

public class KinetoPlusDbContext(DbContextOptions<KinetoPlusDbContext> options) : DbContext(options)
{
    public DbSet<Clinic> Clinics => Set<Clinic>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // SQL Server does not store DateTimeKind, so values read back are marked as UTC.
        modelBuilder.Entity<Clinic>()
            .Property(c => c.CreatedAt)
            .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
    }
}
