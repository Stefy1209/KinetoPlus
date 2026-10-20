using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace WebAPI.Models;

public class Clinic
{
    public Clinic(string name, string address, string city, string? phoneNumber, string? email, string? website)
    {
        if (phoneNumber is not null && !PhoneNumberRegex.IsMatch(phoneNumber))
            throw new ArgumentException("Invalid phone number.", nameof(phoneNumber));
        if (email is not null && !MailAddress.TryCreate(email, out _))
            throw new ArgumentException("Invalid email address.", nameof(email));
        if (website is not null && !IsHttpUrl(website))
            throw new ArgumentException("Invalid website URL.", nameof(website));

        Name = name;
        Address = address;
        City = city;
        PhoneNumber = phoneNumber;
        Email = email;
        Website = website;
        CreatedAt = DateTime.UtcNow;
    }

    // Used by EF Core when loading clinics, so stored rows are not validated again.
    private Clinic()
    {
        Name = null!;
        Address = null!;
        City = null!;
    }

    public Guid Id { get; private set; }
    [MaxLength(200)] public string Name { get; private set; }
    [MaxLength(300)] public string Address { get; private set; }
    [MaxLength(100)] public string City { get; private set; }
    [MaxLength(30)] public string? PhoneNumber { get; private set; }
    [MaxLength(254)] public string? Email { get; private set; }
    [MaxLength(500)] public string? Website { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private static bool IsHttpUrl(string value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    private static readonly Regex PhoneNumberRegex = new(@"^\+?[0-9 ()-]{6,30}$", RegexOptions.Compiled);
}
