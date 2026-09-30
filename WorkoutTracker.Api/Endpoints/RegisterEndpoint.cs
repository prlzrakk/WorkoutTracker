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
                    return Results.BadRequest(new
                    {
                        errors = new[] { "Passwords do not match" }
                    });
                }
                
                try
                {
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

                        return Results.BadRequest(new
                        {
                            errors = result.Errors.Select(error => error.Description)
                        });
                    }

                    logger.LogInformation("Account created. UserId={UserId}, Email={Email}", user.Id, user.Email);
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Registration failed unexpectedly. Email={Email}", dto.Email);

                    return Results.Problem(
                        title: "Registration is temporarily unavailable. Please try again later.",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
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
