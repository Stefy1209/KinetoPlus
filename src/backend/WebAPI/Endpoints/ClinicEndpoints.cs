using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Services;

namespace WebAPI.Endpoints;

/// <summary>
/// Maps the clinic endpoints under <c>/api/clinics</c>.
/// </summary>
public static class ClinicEndpoints
{
    /// <summary>
    /// Adds the clinic endpoints to the application.
    /// </summary>
    /// <param name="app">The route builder to add the endpoints to.</param>
    /// <returns>The same route builder, so calls can be chained.</returns>
    public static IEndpointRouteBuilder MapClinicEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clinics").WithTags("Clinics");

        group.MapGet("/", GetAllAsync)
            .WithName("GetClinics")
            .WithSummary("Gets every clinic, ordered by name.");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetClinicById")
            .WithSummary("Gets the clinic with the given id.");

        return app;
    }

    /// <summary>
    /// Handles <c>GET /api/clinics</c>.
    /// </summary>
    /// <param name="clinicService">The service that reads the clinics.</param>
    /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
    /// <returns>200 OK with every clinic, or an empty list if there are none.</returns>
    public static async Task<Ok<List<ClinicResponse>>> GetAllAsync(IClinicService clinicService, CancellationToken cancellationToken)
    {
        var clinics = await clinicService.GetAllAsync(cancellationToken);
        return TypedResults.Ok(clinics.Select(ClinicResponse.FromClinic).ToList());
    }

    /// <summary>
    /// Handles <c>GET /api/clinics/{id}</c>.
    /// </summary>
    /// <param name="id">The identifier of the clinic.</param>
    /// <param name="clinicService">The service that reads the clinic.</param>
    /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
    /// <returns>200 OK with the clinic, or 404 Not Found if no clinic has that id.</returns>
    public static async Task<Results<Ok<ClinicResponse>, NotFound>> GetByIdAsync(Guid id, IClinicService clinicService, CancellationToken cancellationToken)
    {
        var clinic = await clinicService.GetByIdAsync(id, cancellationToken);
        return clinic is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(ClinicResponse.FromClinic(clinic));
    }
}
