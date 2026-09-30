using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using WorkoutTracker.Client.Models;

namespace WorkoutTracker.Client.Identity;

public class CookieAuthenticationStateProvider(HttpClient http) : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var response = await http.GetAsync("/api/profile");
            if (!response.IsSuccessStatusCode)
                return new AuthenticationState(Anonymous);

            var userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();
            if (userInfo is null)
                return new AuthenticationState(Anonymous);

            var displayName = !string.IsNullOrWhiteSpace(userInfo.Name)
                ? userInfo.Name
                : userInfo.Email;

            if (string.IsNullOrWhiteSpace(displayName))
                return new AuthenticationState(Anonymous);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, displayName),
            };

            if (!string.IsNullOrWhiteSpace(userInfo.Email))
                claims.Add(new Claim(ClaimTypes.Email, userInfo.Email));

            var identity = new ClaimsIdentity(claims, "Identity.Application");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(Anonymous);
        }
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            var response = await http.PostAsJsonAsync("login?useCookies=true",
                new
                {
                    email,
                    password
                });

            if (!response.IsSuccessStatusCode)
            {
                var error = response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    ? "Invalid email or password"
                    : await ReadErrorAsync(response, "Login failed");

                return AuthResult.Fail(error);
            }
        }
        catch
        {
            return AuthResult.Fail("Could not connect to the server");
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return AuthResult.Ok();
    }

    public async Task LogoutAsync()
    {
        try
        {
            await http.PostAsJsonAsync("logout", new { });
        }
        catch
        {
            // If the API is unavailable, still clear the client auth state.
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Anonymous)));
    }

    public async Task<AuthResult> RegisterAsync(
        string name,
        string email,
        string password,
        string passwordConfirmation)
    {
        try
        {
            var response = await http.PostAsJsonAsync("api/register",
                new
                {
                    name,
                    email,
                    password,
                    passwordConfirmation
                });

            if (response.IsSuccessStatusCode)
                return AuthResult.Ok();

            var error = await ReadErrorAsync(response, "Registration failed");
            return AuthResult.Fail(error);
        }
        catch
        {
            return AuthResult.Fail("Could not connect to the server");
        }
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, string fallbackMessage)
    {
        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content))
            return fallbackMessage;

        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            if (root.TryGetProperty("errors", out var errors))
            {
                var errorMessages = ReadErrorMessages(errors);
                if (errorMessages.Count > 0)
                    return string.Join(" ", errorMessages);
            }

            if (root.TryGetProperty("error", out var error) &&
                error.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(error.GetString()))
            {
                return error.GetString()!;
            }

            if (root.TryGetProperty("title", out var title) &&
                title.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(title.GetString()))
            {
                return title.GetString()!;
            }
        }
        catch (JsonException)
        {
            // Fall back to a JSON string or the raw response text below.
        }

        try
        {
            var errorText = JsonSerializer.Deserialize<string>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!string.IsNullOrWhiteSpace(errorText))
                return errorText;
        }
        catch (JsonException)
        {
            // Fall back to the raw response text below.
        }

        var trimmedContent = content.Trim();
        if (LooksLikeTechnicalError(trimmedContent))
            return fallbackMessage;

        return trimmedContent;
    }

    private static bool LooksLikeTechnicalError(string content)
    {
        return content.Length > 300 ||
               content.Contains("Exception", StringComparison.OrdinalIgnoreCase) ||
               content.Contains(" at ", StringComparison.OrdinalIgnoreCase) ||
               content.Contains("<html", StringComparison.OrdinalIgnoreCase) ||
               content.Contains("<!DOCTYPE", StringComparison.OrdinalIgnoreCase);
    }

    private static List<string> ReadErrorMessages(JsonElement errors)
    {
        var messages = new List<string>();

        if (errors.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in errors.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(item.GetString()))
                    messages.Add(item.GetString()!);
            }
        }

        if (errors.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in errors.EnumerateObject())
            {
                if (property.Value.ValueKind != JsonValueKind.Array)
                    continue;

                foreach (var item in property.Value.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(item.GetString()))
                        messages.Add(item.GetString()!);
                }
            }
        }

        return messages;
    }

    public sealed record AuthResult(bool Success, string? ErrorMessage)
    {
        public static AuthResult Ok() => new(true, null);

        public static AuthResult Fail(string errorMessage) => new(false, errorMessage);
    }
}
