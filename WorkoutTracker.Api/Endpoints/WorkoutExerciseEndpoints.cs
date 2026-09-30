using System.Security.Claims;
using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Api.Extensions;
using WorkoutTracker.Core.DTOs.Exercise;
using WorkoutTracker.Core.DTOs.WorkoutExercise;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Api.Endpoints;

public static class WorkoutExerciseEndpoints
{
    public static RouteGroupBuilder MapWorkoutExercise(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IWorkoutExerciseService service, ClaimsPrincipal user, Guid workoutId) =>
        {
            var result = await service.GetWorkoutExercisesAsync(user.GetUserId(), workoutId);
            return Results.Ok(result);
        });

        group.MapDelete("/{workoutExerciseId:guid}",
            async (IWorkoutExerciseService service, ClaimsPrincipal user, Guid workoutId, Guid workoutExerciseId) =>
            {
                var deleted = await service.DeleteWorkoutExerciseAsync(user.GetUserId(), workoutId, workoutExerciseId);
                return deleted ? Results.NoContent() : Results.NotFound();
            });

        group.MapPost("/",
            async (IWorkoutExerciseService service, CreateWorkoutExerciseDto dto, ClaimsPrincipal user, Guid workoutId) =>
            {
                try
                {
                    var result = await service.CreateWorkoutExerciseAsync(user.GetUserId(), workoutId, dto);
                    if (result is null)
                        return Results.NotFound();
                    return Results.Created($"/api/workouts/{workoutId}/exercises/{result.Id}", result);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { error = ex.Message });
                }
            });

        group.MapPatch("/{workoutExerciseId:guid}", async (IWorkoutExerciseService service, Guid workoutId,
            Guid workoutExerciseId, UpdateWorkoutExerciseDto updatedDto, ClaimsPrincipal user) =>
        {
            try
            {
                var updated = await service.UpdateWorkoutExerciseAsync(user.GetUserId(), workoutId, workoutExerciseId, updatedDto);
                return updated ? Results.NoContent() : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
        });

        return group;
    }
}