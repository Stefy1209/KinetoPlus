using WebAPI.Models;
using WebAPI.Repositories;
using WebAPI.Services;

namespace WebAPI.Tests.Services;

// Uses a fake repository so the service is tested on its own, without a database.
public sealed class ClinicServiceTests
{
    // The database normally assigns Id on save. With no database here, each clinic
    // gets its own Id so lookups by Id can tell clinics apart.
    private static Clinic NewClinic(string name)
    {
        var clinic = new Clinic(name, "Strada Lunga 10", "Cluj-Napoca", null, null, null);
        typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, Guid.NewGuid());
        return clinic;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenRepositoryHasNoClinics()
    {
        var service = new ClinicService(new FakeClinicRepository());

        var clinics = await service.GetAllAsync();

        Assert.Empty(clinics);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsClinicsFromRepository_InTheSameOrder()
    {
        var first = NewClinic("Arena Kineto");
        var second = NewClinic("Kineto Center");
        var service = new ClinicService(new FakeClinicRepository(first, second));

        var clinics = await service.GetAllAsync();

        Assert.Equal([first, second], clinics);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMatchingClinic()
    {
        var wanted = NewClinic("Kineto Center");
        var service = new ClinicService(new FakeClinicRepository(NewClinic("Kineto Plus"), wanted));

        var clinic = await service.GetByIdAsync(wanted.Id);

        Assert.Same(wanted, clinic);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNoClinicHasTheId()
    {
        var service = new ClinicService(new FakeClinicRepository(NewClinic("Kineto Center")));

        var clinic = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(clinic);
    }

    [Fact]
    public async Task GetByIdAsync_PassesIdToRepository()
    {
        var repository = new FakeClinicRepository();
        var service = new ClinicService(repository);
        var id = Guid.NewGuid();

        await service.GetByIdAsync(id);

        Assert.Equal(id, repository.LastRequestedId);
    }

    [Fact]
    public async Task Methods_PassCancellationTokenToRepository()
    {
        var repository = new FakeClinicRepository();
        var service = new ClinicService(repository);
        using var source = new CancellationTokenSource();

        await service.GetAllAsync(source.Token);
        Assert.Equal(source.Token, repository.LastToken);

        await service.GetByIdAsync(Guid.NewGuid(), source.Token);
        Assert.Equal(source.Token, repository.LastToken);
    }

    [Fact]
    public async Task Methods_StopWhenCancelled()
    {
        var service = new ClinicService(new FakeClinicRepository());
        var cancelled = new CancellationToken(canceled: true);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.GetAllAsync(cancelled));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.GetByIdAsync(Guid.NewGuid(), cancelled));
    }

    // Keeps clinics in memory and records what the service asked for.
    private sealed class FakeClinicRepository(params Clinic[] clinics) : IClinicRepository
    {
        public Guid? LastRequestedId { get; private set; }
        public CancellationToken LastToken { get; private set; }

        public Task<IReadOnlyList<Clinic>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            LastToken = cancellationToken;
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<IReadOnlyList<Clinic>>(cancellationToken);
            return Task.FromResult<IReadOnlyList<Clinic>>(clinics);
        }

        public Task<Clinic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            LastRequestedId = id;
            LastToken = cancellationToken;
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<Clinic?>(cancellationToken);
            return Task.FromResult(clinics.FirstOrDefault(c => c.Id == id));
        }
    }
}
