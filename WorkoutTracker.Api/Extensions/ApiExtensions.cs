using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WorkoutTracker.Api.Endpoints;
using WorkoutTracker.Endpoints;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Api.Extensions;

public static class ApiExtensions
{
    public static WebApplicationBuilder AddProjectDatabase(this WebApplicationBuilder builder)
    {
        var host = builder.Configuration["DB_HOST"];
        var port = builder.Configuration["DB_PORT"];
        var db = builder.Configuration["DB_NAME"];
        var user = builder.Configuration["DB_USER"];
        var pass = builder.Configuration["DB_PASSWORD"];
        var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={pass}";
        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(port) ||
            string.IsNullOrWhiteSpace(db) ||
            string.IsNullOrWhiteSpace(user) ||
            string.IsNullOrWhiteSpace(pass))
        {
            throw new InvalidOperationException("Database environment variables are not configured");
        }

        builder.Services.AddDbContext<ProjectContext>(options => { options.UseNpgsql(connectionString); });

        return builder;
    }

    public static WebApplicationBuilder LoadEnvFiles(this WebApplicationBuilder builder)
    {
        DotNetEnv.Env.NoClobber().Load("../.env");
        DotNetEnv.Env.NoClobber().Load();
        builder.Configuration.AddEnvironmentVariables();

        return builder;
    }

    public static WebApplicationBuilder AddClientCors(this WebApplicationBuilder builder)
    {
        var clientUrl = builder.Configuration["CLIENT_URL"];
        var clientHttpsUrl = builder.Configuration["CLIENT_HTTPS_URL"];

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("ClientApp", policy =>
                policy.WithOrigins(clientUrl!, clientHttpsUrl!)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });
        return builder;
    }

    public static WebApplicationBuilder AddIdentityAuth(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddEntityFrameworkStores<ProjectContext>()
            .AddApiEndpoints();

        builder.Services.AddAuthorization();

        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<HashEmailService>();
        builder.Services.AddOpenApi();
        builder.Services.AddScoped<IExerciseService, ExerciseService>();
        builder.Services.AddScoped<IWorkoutService, WorkoutService>();
        builder.Services.AddScoped<ISetService, SetService>();
        builder.Services.AddScoped<IWorkoutExerciseService, WorkoutExerciseService>();

        return builder;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapRegisterEndpoint();
        app.MapGroup("api/exercises").RequireAuthorization().MapExerciseEndpoints();
        app.MapGroup("/api/workouts/{workoutId:guid}/exercises/{workoutExerciseId:guid}/sets").RequireAuthorization()
            .MapSets();
        app.MapGroup("/api/workouts").RequireAuthorization().MapWorkouts();
        app.MapGroup("api/workouts/{workoutId:guid}/exercises").RequireAuthorization().MapWorkoutExercise();

        app.MapIdentityApi<ApplicationUser>();
        return app;
    }

    public static WebApplication UseApiMiddleware(this WebApplication app)
    {
        app.UseCors("ClientApp");
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication MapApiDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        return app;
    }
}