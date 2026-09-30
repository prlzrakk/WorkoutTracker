using System.Security.Claims;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using WorkoutTracker.Core.DTOs.Register;

namespace WorkoutTracker.Api.Endpoints;

public static class RegisterEndpoint
{
    public static WebApplication MapRegisterEndpoint(this WebApplication app)
    {
        app.MapPost("/api/register",
            async (RegisterDto dto, UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("RegisterEndpoint");

                if (dto.Password != dto.PasswordConfirmation)
                {
                    logger.LogWarning("Registration failed. Reason=PasswordMismatch, Email={Email}", dto.Email);
                    return Results.BadRequest("Passwords do not match");
                }
                
                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    Name = dto.Name
                };
                var result = await userManager.CreateAsync(user, dto.Password);
                
                if (!result.Succeeded)
                {
                    logger.LogWarning(
                        "Registration failed. Email={Email}, Errors={Errors}",
                        dto.Email,
                        string.Join(", ", result.Errors.Select(error => error.Code)));

                    return Results.BadRequest(result.Errors);
                }

                logger.LogInformation("Account created. UserId={UserId}, Email={Email}", user.Id, user.Email);
                return Results.Ok();
            }).AllowAnonymous();

        app.MapGet("/api/profile", async (ClaimsPrincipal user, UserManager<ApplicationUser> userManager) =>
        {
            var currentUser = await userManager.GetUserAsync(user);
            return currentUser is null
                ? Results.Unauthorized()
                : Results.Ok(new
                {
                    currentUser.Name,
                    Email = currentUser.Email ?? currentUser.UserName ?? string.Empty
                });
        }).RequireAuthorization();

        app.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.Ok();
            })
            .RequireAuthorization();

        return app;
    }
}
