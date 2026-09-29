using System.Security.Claims;

namespace WorkoutTracker.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new UnauthorizedAccessException();
    }
}