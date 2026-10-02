using System.Security.Cryptography;
using System.Text;

namespace WorkoutTracker.Services;

public class HashEmailService
{
    public string ReturnHashedEmail(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var bytes = Encoding.UTF8.GetBytes(normalizedEmail);
        var hashBytes = SHA256.HashData(bytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}