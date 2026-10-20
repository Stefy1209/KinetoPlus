using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Tests.Data;

public class KinetoPlusDbContextTests
{
    // No database is reached: these tests only inspect the SQL Server model and the migrations.
    private static KinetoPlusDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<KinetoPlusDbContext>()
            .UseSqlServer("Server=unused;Database=unused")
            .Options);

    private static IEntityType ClinicEntity(KinetoPlusDbContext context) =>
        context.Model.FindEntityType(typeof(Clinic))!;

    [Fact]
    public void Clinic_IsMappedToClinicsTable()
    {
        using var context = CreateContext();

        Assert.Equal("Clinics", ClinicEntity(context).GetTableName());
    }

    [Fact]
    public void Clinic_HasOnlyTheDesignedColumns()
    {
        using var context = CreateContext();

        var columns = ClinicEntity(context).GetProperties().Select(p => p.Name).Order();

        Assert.Equal(
            ["Address", "City", "CreatedAt", "Email", "Id", "Name", "PhoneNumber", "Website"],
            columns);
    }

    [Fact]
    public void Id_IsGuidPrimaryKeyGeneratedOnAdd()
    {
        using var context = CreateContext();
        var id = ClinicEntity(context).FindProperty(nameof(Clinic.Id))!;

        Assert.True(id.IsPrimaryKey());
        Assert.Equal("uniqueidentifier", id.GetColumnType());
        Assert.Equal(ValueGenerated.OnAdd, id.ValueGenerated);
    }

    [Fact]
    public void Id_IsGeneratedByEfCoreWhenClinicIsAdded()
    {
        using var context = CreateContext();
        var first = new Clinic("Kineto Center", "Strada Lunga 10", "Cluj-Napoca", null, null, null);
        var second = new Clinic("Kineto Plus", "Strada Scurta 2", "Cluj-Napoca", null, null, null);

        context.Clinics.AddRange(first, second);

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(Guid.Empty, second.Id);
        Assert.NotEqual(first.Id, second.Id);
    }

    [Theory]
    [InlineData(nameof(Clinic.Name), "nvarchar(200)", false)]
    [InlineData(nameof(Clinic.Address), "nvarchar(300)", false)]
    [InlineData(nameof(Clinic.City), "nvarchar(100)", false)]
    [InlineData(nameof(Clinic.PhoneNumber), "nvarchar(30)", true)]
    [InlineData(nameof(Clinic.Email), "nvarchar(254)", true)]
    [InlineData(nameof(Clinic.Website), "nvarchar(500)", true)]
    [InlineData(nameof(Clinic.CreatedAt), "datetime2", false)]
    public void Column_HasDesignedTypeAndNullability(string propertyName, string columnType, bool isNullable)
    {
        using var context = CreateContext();
        var property = ClinicEntity(context).FindProperty(propertyName)!;

        Assert.Equal(columnType, property.GetColumnType());
        Assert.Equal(isNullable, property.IsNullable);
    }

    [Fact]
    public void CreatedAt_IsReadBackAsUtc()
    {
        using var context = CreateContext();
        var converter = ClinicEntity(context).FindProperty(nameof(Clinic.CreatedAt))!.GetValueConverter()!;
        var stored = new DateTime(2026, 10, 20, 10, 0, 0, DateTimeKind.Unspecified);

        var read = (DateTime)converter.ConvertFromProvider(stored)!;

        Assert.Equal(DateTimeKind.Utc, read.Kind);
        Assert.Equal(stored.Ticks, read.Ticks);
    }

    [Fact]
    public void Migrations_ContainInitialCreate()
    {
        using var context = CreateContext();

        Assert.Contains(context.Database.GetMigrations(), m => m.EndsWith("_InitialCreate"));
    }

    [Fact]
    public void Migrations_MatchTheCurrentModel()
    {
        using var context = CreateContext();

        Assert.False(
            context.Database.HasPendingModelChanges(),
            "The model has changes that are not in a migration. Run 'dotnet ef migrations add <Name>'.");
    }
}
