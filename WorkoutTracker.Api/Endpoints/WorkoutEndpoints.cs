using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Core.DTOs.Workout;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Api.Endpoints;

public static class WorkoutEndpoints
{
    public static RouteGroupBuilder MapWorkouts(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IWorkoutService service) =>
        {
            var result = await service.GetWorkoutsAsync();
            return Results.Ok(result);
        });

        group.MapGet("/{workoutId:guid}", async (IWorkoutService service, Guid workoutId) =>
        {
            var workoutDto = await service.GetWorkoutByIdAsync(workoutId);
            return workoutDto == null ? Results.NotFound() : Results.Ok(workoutDto);
        });

        group.MapDelete("/{workoutId:guid}", async (IWorkoutService service, Guid workoutId) =>
        {
            var deleted = await service.DeleteWorkoutAsync(workoutId);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        group.MapPatch("/{workoutId:guid}",
            async (IWorkoutService service, Guid workoutId, UpdateWorkoutDto updatedWorkout) =>
            {
                var updated = await service.UpdateWorkoutAsync(workoutId, updatedWorkout);
                return updated ? Results.NoContent() : Results.NotFound();
            });

        group.MapPost("/", async (IWorkoutService service, CreateWorkoutDto createdWorkout) =>
        {
            var result = await service.CreateWorkoutAsync(createdWorkout);
            return Results.Created($"api/workouts/{result.workoutId}", result);
        });


        return group;
    }
}