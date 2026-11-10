using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services;

/// <summary>
/// Serves clinic requests by reading clinics through <see cref="IClinicRepository"/>.
/// </summary>
public class ClinicService(IClinicRepository clinicRepository) : IClinicService
{   public Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => clinicRepository.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<Clinic>> GetAllAsync(CancellationToken cancellationToken = default)
        => clinicRepository.GetAllAsync(cancellationToken);
}
