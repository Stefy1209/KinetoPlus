using WebAPI.Models;

namespace WebAPI.Repositories;

/// <summary>
/// Provides access to the <see cref="Clinic"/> records stored in the database.
/// </summary>
public interface IClinicRepository
{
    /// <summary>
    /// Gets the clinic with the given identifier.
    /// </summary>
    /// <param name="id">The identifier of the clinic to look up.</param>
    /// <param name="cancellationToken">A token to cancel the database query.</param>
    /// <returns>The matching clinic, or <see langword="null"/> if no clinic has that identifier.</returns>
    Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets every clinic stored in the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the database query.</param>
    /// <returns>A read-only list of all clinics, which is empty if there are none.</returns>
    Task<IReadOnlyList<Clinic>> GetAllAsync(CancellationToken cancellationToken = default);
}
