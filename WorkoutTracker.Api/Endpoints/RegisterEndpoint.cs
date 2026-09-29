using System.Security.Claims;
using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using WorkoutTracker.Core.DTOs.Register;

namespace WorkoutTracker.Api.Endpoints;

public static class RegisterEndpoint
{
    public static WebApplication MapRegisterEndpoint(this WebApplication app)
    {
        app.MapPost("/api/register",
            async (RegisterDto dto, ProjectContext context, UserManager<ApplicationUser> userManager) =>
            {
                if (dto.Password != dto.PasswordConfirmation)
                    return Results.BadRequest("Passwords do not match");
                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    Name = dto.Name
                };
                var result = await userManager.CreateAsync(user, dto.Password);
                return result.Succeeded
                    ? Results.Ok()
                    : Results.BadRequest(result.Errors);
            }).AllowAnonymous();
        
        app.MapGet("/api/profile", async (ClaimsPrincipal user, UserManager<ApplicationUser> userManager) =>
        {
            var currentUser = await userManager.GetUserAsync(user);
            return currentUser is null ? Results.Unauthorized() : Results.Ok(new {currentUser.Name});
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