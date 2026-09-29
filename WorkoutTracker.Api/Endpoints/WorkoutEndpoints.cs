using WorkoutTracker.Core.DTOs.Workout;
using WorkoutTracker.Services.Interfaces;
using System.Security.Claims;
using WorkoutTracker.Api.Extensions;

namespace WorkoutTracker.Api.Endpoints;

public static class WorkoutEndpoints
{
    public static RouteGroupBuilder MapWorkouts(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IWorkoutService service, ClaimsPrincipal user) =>
        {
            var result = await service.GetWorkoutsAsync(user.GetUserId());
            return Results.Ok(result);
        });

        group.MapGet("/{workoutId:guid}", async (IWorkoutService service, ClaimsPrincipal user, Guid workoutId) =>
        {
            var workoutDto = await service.GetWorkoutByIdAsync(user.GetUserId(), workoutId);
            return workoutDto == null ? Results.NotFound() : Results.Ok(workoutDto);
        });

        group.MapDelete("/{workoutId:guid}", async (IWorkoutService service, ClaimsPrincipal user, Guid workoutId) =>
        {
            var deleted = await service.DeleteWorkoutAsync(user.GetUserId(), workoutId);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        group.MapPatch("/{workoutId:guid}",
            async (IWorkoutService service, Guid workoutId, ClaimsPrincipal user, UpdateWorkoutDto updatedWorkout) =>
            {
                var updated = await service.UpdateWorkoutAsync(user.GetUserId(), workoutId, updatedWorkout);
                return updated ? Results.NoContent() : Results.NotFound();
            });

        group.MapPost("/", async (IWorkoutService service, ClaimsPrincipal user, CreateWorkoutDto createdWorkout) =>
        {
            var result = await service.CreateWorkoutAsync(user.GetUserId(), createdWorkout);
            return Results.Created($"api/workouts/{result.workoutId}", result);
        });


        return group;
    }
}