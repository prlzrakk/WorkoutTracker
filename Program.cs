using Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Api.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ProjectContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGroup("api/exercises").MapExerciseEndpoints();
app.MapGroup("/api/workouts/{workoutId:guid}/exercises/{workoutExerciseId:guid}/sets").MapSets();
app.MapGroup("/api/workouts").MapWorkouts();
app.MapGroup("api/workouts/{workoutId:guid}/exercises").MapWorkoutExercise();

app.Run();

