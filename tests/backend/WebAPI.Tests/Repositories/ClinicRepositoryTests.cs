using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Tests.Repositories;

// Uses an in-memory SQLite database so the repository runs real queries.
public sealed class ClinicRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public ClinicRepositoryTests()
    {
        _connection.Open();
        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private KinetoPlusDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<KinetoPlusDbContext>()
            .UseSqlite(_connection)
            .Options);

    private void Seed(params Clinic[] clinics)
    {
        using var context = CreateContext();
        context.Clinics.AddRange(clinics);
        context.SaveChanges();
    }

    private static Clinic NewClinic(string name) =>
        new(name, "Strada Lunga 10", "Cluj-Napoca", null, null, null);

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenThereAreNoClinics()
    {
        using var context = CreateContext();
        var repository = new ClinicRepository(context);

        var clinics = await repository.GetAllAsync();

        Assert.Empty(clinics);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllClinics_OrderedByName()
    {
        Seed(NewClinic("Kineto Plus"), NewClinic("Arena Kineto"), NewClinic("Kineto Center"));
        using var context = CreateContext();
        var repository = new ClinicRepository(context);

        var clinics = await repository.GetAllAsync();

        Assert.Equal(["Arena Kineto", "Kineto Center", "Kineto Plus"], clinics.Select(c => c.Name));
    }

    [Fact]
    public async Task GetAllAsync_DoesNotTrackClinics()
    {
        Seed(NewClinic("Kineto Center"));
        using var context = CreateContext();
        var repository = new ClinicRepository(context);

        await repository.GetAllAsync();

        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMatchingClinic()
    {
        var wanted = NewClinic("Kineto Center");
        Seed(NewClinic("Kineto Plus"), wanted);
        using var context = CreateContext();
        var repository = new ClinicRepository(context);

        var clinic = await repository.GetByIdAsync(wanted.Id);

        Assert.NotNull(clinic);
        Assert.Equal(wanted.Id, clinic.Id);
        Assert.Equal("Kineto Center", clinic.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNoClinicHasTheId()
    {
        Seed(NewClinic("Kineto Center"));
        using var context = CreateContext();
        var repository = new ClinicRepository(context);

        var clinic = await repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(clinic);
    }

    [Fact]
    public async Task GetByIdAsync_DoesNotTrackClinic()
    {
        var wanted = NewClinic("Kineto Center");
        Seed(wanted);
        using var context = CreateContext();
        var repository = new ClinicRepository(context);

        await repository.GetByIdAsync(wanted.Id);

        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task Methods_StopWhenCancelled()
    {
        using var context = CreateContext();
        var repository = new ClinicRepository(context);
        var cancelled = new CancellationToken(canceled: true);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => repository.GetAllAsync(cancelled));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => repository.GetByIdAsync(Guid.NewGuid(), cancelled));
    }
}
