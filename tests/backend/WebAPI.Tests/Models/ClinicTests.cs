using WebAPI.Models;

namespace WebAPI.Tests.Models;

public class ClinicTests
{
    private static Clinic CreateClinic(string? phoneNumber = null, string? email = null, string? website = null) =>
        new("Kineto Center", "Strada Lunga 10", "Cluj-Napoca", phoneNumber, email, website);

    [Fact]
    public void Constructor_SetsAllValues()
    {
        var before = DateTime.UtcNow;

        var clinic = new Clinic(
            name: "Kineto Center", 
            address: "Strada Lunga 10", 
            city: "Cluj-Napoca",
            phoneNumber: "+40 721 234 567", 
            email: "contact@kineto.ro", 
            website: "https://kineto.ro");

        Assert.Equal("Kineto Center", clinic.Name);
        Assert.Equal("Strada Lunga 10", clinic.Address);
        Assert.Equal("Cluj-Napoca", clinic.City);
        Assert.Equal("+40 721 234 567", clinic.PhoneNumber);
        Assert.Equal("contact@kineto.ro", clinic.Email);
        Assert.Equal("https://kineto.ro", clinic.Website);
        Assert.Equal(DateTimeKind.Utc, clinic.CreatedAt.Kind);
        Assert.InRange(clinic.CreatedAt, before, DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_AllowsMissingContactDetails()
    {
        var clinic = CreateClinic();

        Assert.Null(clinic.PhoneNumber);
        Assert.Null(clinic.Email);
        Assert.Null(clinic.Website);
    }

    [Theory]
    [InlineData("+40 721 234 567")]
    [InlineData("0721234567")]
    [InlineData("(021) 123-4567")]
    public void Constructor_AcceptsValidPhoneNumber(string phoneNumber)
    {
        Assert.Equal(phoneNumber, CreateClinic(phoneNumber: phoneNumber).PhoneNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("0721 ABC 567")]
    [InlineData("0040+721234567")]
    public void Constructor_RejectsInvalidPhoneNumber(string phoneNumber)
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateClinic(phoneNumber: phoneNumber));
        Assert.Equal("phoneNumber", exception.ParamName);
    }

    [Theory]
    [InlineData("contact@kineto.ro")]
    [InlineData("john.doe@clinic.example.com")]
    public void Constructor_AcceptsValidEmail(string email)
    {
        Assert.Equal(email, CreateClinic(email: email).Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("contact@")]
    [InlineData("@kineto.ro")]
    public void Constructor_RejectsInvalidEmail(string email)
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateClinic(email: email));
        Assert.Equal("email", exception.ParamName);
    }

    [Theory]
    [InlineData("https://kineto.ro")]
    [InlineData("http://kineto.ro/contact")]
    public void Constructor_AcceptsValidWebsite(string website)
    {
        Assert.Equal(website, CreateClinic(website: website).Website);
    }

    [Theory]
    [InlineData("")]
    [InlineData("kineto.ro")]
    [InlineData("ftp://kineto.ro")]
    [InlineData("not a url")]
    public void Constructor_RejectsInvalidWebsite(string website)
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateClinic(website: website));
        Assert.Equal("website", exception.ParamName);
    }

    [Fact]
    public void Properties_CannotBeChangedFromOutside()
    {
        foreach (var property in typeof(Clinic).GetProperties())
            Assert.False(property.SetMethod?.IsPublic ?? false, $"{property.Name} has a public setter.");
    }
}
