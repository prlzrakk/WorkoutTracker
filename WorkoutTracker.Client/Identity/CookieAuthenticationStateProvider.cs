using System.Net.Http.Json;
using System.Security.Claims;
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

    public async Task<bool> LoginAsync(string email, string password)
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
                return false;
        }
        catch
        {
            return false;
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return true;
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

    public async Task<bool> RegisterAsync(
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
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
