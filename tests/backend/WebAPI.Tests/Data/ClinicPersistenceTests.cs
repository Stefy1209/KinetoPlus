using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Tests.Data;

// Uses an in-memory SQLite database to check that clinics are saved and loaded correctly.
public sealed class ClinicPersistenceTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public ClinicPersistenceTests()
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

    [Fact]
    public void SavedClinic_IsLoadedWithAllValues()
    {
        var clinic = new Clinic(
            name: "Kineto Center",
            address: "Strada Lunga 10",
            city: "Cluj-Napoca",
            phoneNumber: "+40 721 234 567",
            email: "contact@kineto.ro",
            website: "https://kineto.ro");
        using (var context = CreateContext())
        {
            context.Clinics.Add(clinic);
            context.SaveChanges();
        }

        using var readContext = CreateContext();
        var loaded = readContext.Clinics.Single();

        Assert.Equal(clinic.Id, loaded.Id);
        Assert.Equal("Kineto Center", loaded.Name);
        Assert.Equal("Strada Lunga 10", loaded.Address);
        Assert.Equal("Cluj-Napoca", loaded.City);
        Assert.Equal("+40 721 234 567", loaded.PhoneNumber);
        Assert.Equal("contact@kineto.ro", loaded.Email);
        Assert.Equal("https://kineto.ro", loaded.Website);
        Assert.Equal(clinic.CreatedAt, loaded.CreatedAt);
        Assert.Equal(DateTimeKind.Utc, loaded.CreatedAt.Kind);
    }

    [Fact]
    public void LoadingClinic_KeepsStoredCreatedAt()
    {
        var id = Guid.NewGuid().ToString().ToUpperInvariant();
        using (var context = CreateContext())
        {
            context.Database.ExecuteSqlRaw(
                "INSERT INTO Clinics (Id, Name, Address, City, CreatedAt) " +
                "VALUES ({0}, 'Kineto Center', 'Strada Lunga 10', 'Cluj-Napoca', '2026-01-15 08:30:00')",
                id);
        }

        using var readContext = CreateContext();
        var loaded = readContext.Clinics.Single();

        Assert.Equal(new DateTime(2026, 1, 15, 8, 30, 0, DateTimeKind.Utc), loaded.CreatedAt);
        Assert.Equal(DateTimeKind.Utc, loaded.CreatedAt.Kind);
    }

    [Fact]
    public void LoadingClinic_DoesNotValidateStoredValuesAgain()
    {
        var id = Guid.NewGuid().ToString().ToUpperInvariant();
        using (var context = CreateContext())
        {
            context.Database.ExecuteSqlRaw(
                "INSERT INTO Clinics (Id, Name, Address, City, Email, CreatedAt) " +
                "VALUES ({0}, 'Kineto Center', 'Strada Lunga 10', 'Cluj-Napoca', 'not-an-email', '2026-01-15 08:30:00')",
                id);
        }

        using var readContext = CreateContext();
        var loaded = readContext.Clinics.Single();

        Assert.Equal("not-an-email", loaded.Email);
    }
}
