using System.Security.Claims;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using WorkoutTracker.Core.DTOs.Register;
using WorkoutTracker.Services;

namespace WorkoutTracker.Api.Endpoints;

public static class RegisterEndpoint
{
    public static WebApplication MapRegisterEndpoint(this WebApplication app)
    {
        app.MapPost("/api/register",
            async (RegisterDto dto, UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory, HashEmailService hashEmail) =>
            {
                var logger = loggerFactory.CreateLogger("RegisterEndpoint");
                var hashedEmail = hashEmail.ReturnHashedEmail(dto.Email);

                if (dto.Password != dto.PasswordConfirmation)
                {
                    logger.LogWarning("Registration failed. Reason=PasswordMismatch EmailHash={hashedEmail}", hashedEmail);
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
                            "Registration failed. EmailHash={hashedEmail}, Errors={Errors}",
                            hashedEmail,
                            string.Join(", ", result.Errors.Select(error => error.Code)));

                        return Results.BadRequest(new
                        {
                            errors = result.Errors.Select(error => error.Description)
                        });
                    }

                    logger.LogInformation("Account created. UserId={UserId}, EmailHash={hashedEmail}", user.Id, hashedEmail);
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Registration failed unexpectedly. EmailHash={hashedEmail}", hashedEmail);

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
