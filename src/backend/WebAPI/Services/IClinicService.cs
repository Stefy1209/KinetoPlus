using WebAPI.Models;

namespace WebAPI.Services;

/// <summary>
/// Provides the clinic operations that the API exposes to its callers.
/// </summary>
public interface IClinicService
{
    /// <summary>
    /// Gets the clinic with the given identifier.
    /// </summary>
    /// <param name="id">The identifier of the clinic to look up.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The matching clinic, or <see langword="null"/> if no clinic has that identifier.</returns>
    Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets every clinic.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A read-only list of all clinics, which is empty if there are none.</returns>
    Task<IReadOnlyList<Clinic>> GetAllAsync(CancellationToken cancellationToken = default);
}
