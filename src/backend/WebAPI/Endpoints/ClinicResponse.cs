using WebAPI.Models;

namespace WebAPI.Endpoints;

/// <summary>
/// The clinic data returned by the API.
/// </summary>
/// <param name="Id">The identifier of the clinic.</param>
/// <param name="Name">The clinic name.</param>
/// <param name="Address">The street and number.</param>
/// <param name="City">The city or town.</param>
/// <param name="PhoneNumber">The contact phone, if any.</param>
/// <param name="Email">The contact email, if any.</param>
/// <param name="Website">The clinic website, if any.</param>
/// <param name="CreatedAt">When the clinic was added, in UTC.</param>
public sealed record ClinicResponse(
    Guid Id,
    string Name,
    string Address,
    string City,
    string? PhoneNumber,
    string? Email,
    string? Website,
    DateTime CreatedAt)
{
    /// <summary>
    /// Creates the response for the given clinic.
    /// </summary>
    /// <param name="clinic">The clinic to describe.</param>
    /// <returns>A response with the clinic's values.</returns>
    public static ClinicResponse FromClinic(Clinic clinic) => new(
        clinic.Id,
        clinic.Name,
        clinic.Address,
        clinic.City,
        clinic.PhoneNumber,
        clinic.Email,
        clinic.Website,
        clinic.CreatedAt);
}
