using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Endpoints;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Tests.Endpoints;

// Calls the endpoint handlers directly with a fake service, so no server or database is needed.
public sealed class ClinicEndpointsTests
{
    // The database normally assigns Id on save. With no database here, each clinic
    // gets its own Id so lookups by Id can tell clinics apart.
    private static Clinic NewClinic(string name)
    {
        var clinic = new Clinic(name, "Strada Lunga 10", "Cluj-Napoca", "+40 721 234 567", "contact@kineto.ro", "https://kineto.ro");
        typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, Guid.NewGuid());
        return clinic;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOkWithEmptyList_WhenThereAreNoClinics()
    {
        var result = await ClinicEndpoints.GetAllAsync(new FakeClinicService(), CancellationToken.None);

        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOkWithClinics_InTheServiceOrder()
    {
        var first = NewClinic("Arena Kineto");
        var second = NewClinic("Kineto Center");

        var result = await ClinicEndpoints.GetAllAsync(new FakeClinicService(first, second), CancellationToken.None);

        Assert.Equal([ClinicResponse.FromClinic(first), ClinicResponse.FromClinic(second)], result.Value);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsOkWithClinic_WhenItExists()
    {
        var wanted = NewClinic("Kineto Center");
        var service = new FakeClinicService(NewClinic("Kineto Plus"), wanted);

        var result = await ClinicEndpoints.GetByIdAsync(wanted.Id, service, CancellationToken.None);

        var ok = Assert.IsType<Ok<ClinicResponse>>(result.Result);
        Assert.Equal(ClinicResponse.FromClinic(wanted), ok.Value);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNotFound_WhenNoClinicHasTheId()
    {
        var service = new FakeClinicService(NewClinic("Kineto Center"));

        var result = await ClinicEndpoints.GetByIdAsync(Guid.NewGuid(), service, CancellationToken.None);

        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task Handlers_PassIdAndCancellationTokenToService()
    {
        var service = new FakeClinicService();
        using var source = new CancellationTokenSource();
        var id = Guid.NewGuid();

        await ClinicEndpoints.GetAllAsync(service, source.Token);
        Assert.Equal(source.Token, service.LastToken);

        await ClinicEndpoints.GetByIdAsync(id, service, source.Token);
        Assert.Equal(source.Token, service.LastToken);
        Assert.Equal(id, service.LastRequestedId);
    }

    [Fact]
    public void ClinicResponse_FromClinic_CopiesEveryValue()
    {
        var clinic = NewClinic("Kineto Center");

        var response = ClinicResponse.FromClinic(clinic);

        Assert.Equal(clinic.Id, response.Id);
        Assert.Equal(clinic.Name, response.Name);
        Assert.Equal(clinic.Address, response.Address);
        Assert.Equal(clinic.City, response.City);
        Assert.Equal(clinic.PhoneNumber, response.PhoneNumber);
        Assert.Equal(clinic.Email, response.Email);
        Assert.Equal(clinic.Website, response.Website);
        Assert.Equal(clinic.CreatedAt, response.CreatedAt);
    }

    // Keeps clinics in memory and records what the endpoints asked for.
    private sealed class FakeClinicService(params Clinic[] clinics) : IClinicService
    {
        public Guid? LastRequestedId { get; private set; }
        public CancellationToken LastToken { get; private set; }

        public Task<IReadOnlyList<Clinic>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            LastToken = cancellationToken;
            return Task.FromResult<IReadOnlyList<Clinic>>(clinics);
        }

        public Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            LastRequestedId = id;
            LastToken = cancellationToken;
            return Task.FromResult(clinics.FirstOrDefault(c => c.Id == id));
        }
    }
}
