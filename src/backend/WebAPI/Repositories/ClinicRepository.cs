using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Repositories;

/// <summary>
/// Reads clinics from the database through <see cref="KinetoPlusDbContext"/>.
/// </summary>
public class ClinicRepository(KinetoPlusDbContext context) : IClinicRepository
{
    // Clinics are only read here, so change tracking is turned off to save memory and time.
    public async Task<IReadOnlyList<Clinic>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Clinics
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

    public Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Clinics
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}
