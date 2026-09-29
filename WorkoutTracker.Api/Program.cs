using Infrastructure.Db;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Api.Endpoints;
using Scalar.AspNetCore;
using WorkoutTracker.Endpoints;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientApp", policy =>
        policy.WithOrigins("http://localhost:5202", "https://localhost:7176")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddDbContext<ProjectContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<IWorkoutService, WorkoutService>();
builder.Services.AddScoped<ISetService, SetService>();
builder.Services.AddScoped<IWorkoutExerciseService, WorkoutExerciseService>();
builder.Services
    .AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

builder.Services
    .AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ProjectContext>()
    .AddApiEndpoints();

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors("ClientApp");
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGroup("api/exercises").RequireAuthorization().MapExerciseEndpoints();
app.MapGroup("/api/workouts/{workoutId:guid}/exercises/{workoutExerciseId:guid}/sets").RequireAuthorization().MapSets();
app.MapGroup("/api/workouts").RequireAuthorization().MapWorkouts();
app.MapGroup("api/workouts/{workoutId:guid}/exercises").RequireAuthorization().MapWorkoutExercise();
app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager) =>
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    })
    .RequireAuthorization();
app.MapIdentityApi<IdentityUser>();

app.Run();