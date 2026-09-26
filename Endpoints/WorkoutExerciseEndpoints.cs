using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Core.DTOs.Exercise;
using WorkoutTracker.Core.DTOs.WorkoutExercise;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Api.Endpoints;

public static class WorkoutExerciseEndpoints
{
    public static RouteGroupBuilder MapWorkoutExercise(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IWorkoutExerciseService service, Guid workoutId) =>
        {
            var result = await service.GetWorkoutExercisesAsync(workoutId);
            return Results.Ok(result);
        });

        group.MapDelete("/{workoutExerciseId:guid}",
            async (IWorkoutExerciseService service, Guid workoutId, Guid workoutExerciseId) =>
            {
                var deleted = await service.DeleteWorkoutExerciseAsync(workoutId, workoutExerciseId);
                return deleted ? Results.NoContent() : Results.NotFound();
            });

        group.MapPost("/",
            async (IWorkoutExerciseService service, CreateWorkoutExerciseDto dto, Guid workoutId) =>
            {
                try
                {
                    var result = await service.CreateWorkoutExerciseAsync(workoutId, dto);
                    if (result is null)
                        return Results.NotFound();
                    return Results.Created($"/api/workouts/{workoutId}/exercises/{result.Id}", result);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new {error = ex.Message});
                }
            });

        group.MapPatch("/{workoutExerciseId:guid}", async (IWorkoutExerciseService service, Guid workoutId,
            Guid workoutExerciseId, UpdateWorkoutExerciseDto updatedDto) =>
        {
            try
            {
                var updated = await service.UpdateWorkoutExerciseAsync(workoutId, workoutExerciseId, updatedDto);
                return updated ? Results.NoContent() : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new {error = ex.Message});
            }
        });

        return group;
    }
}