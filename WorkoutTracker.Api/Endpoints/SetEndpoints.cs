using Infrastructure.Db;
using Infrastructure.Entities;
using WorkoutTracker.Core.DTOs.Set;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Endpoints;

public static class SetEndpoints
{
    public static RouteGroupBuilder MapSets(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (ISetService service, Guid workoutId, Guid workoutExerciseId) =>
        {
            var sets = await service.GetSetsAsync(workoutId, workoutExerciseId);
            return Results.Ok(sets);
        });

        group.MapGet("/{setId:guid}", async (ISetService service, Guid setId, Guid workoutId, Guid workoutExerciseId) =>
        {
            var set = await service.GetSetByIdAsync(workoutId, workoutExerciseId, setId);
            return set == null ? Results.NotFound() : Results.Ok(set);
        });

        group.MapPost("/",
            async (ISetService service, CreateSetDto newSet, Guid workoutId, Guid workoutExerciseId) =>
            {
                try
                {
                    var result = await service.CreateSetAsync(workoutId, workoutExerciseId, newSet);
                    if (result is null)
                        return Results.NotFound();
                    return Results.Created(
                        $"/api/workouts/{workoutId}/exercises/{workoutExerciseId}/sets/{result.SetId}",
                        result);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new {error = ex.Message});
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new {error = ex.Message});
                }
            });

        group.MapDelete("/{setId:guid}",
            async (ISetService service, Guid setId, Guid workoutId, Guid workoutExerciseId) =>
            {
                var deleted = await service.DeleteSetAsync(workoutId, workoutExerciseId, setId);
                return deleted ? Results.NoContent() : Results.NotFound();
            });

        group.MapPatch("/{setId:guid}",
            async (ISetService service, Guid setId, Guid workoutId, Guid workoutExerciseId,
                UpdateSetDto updateSetDto) =>
            {
                try
                {
                    var updated = await service.UpdateSetAsync(setId, workoutId, workoutExerciseId, updateSetDto);
                    return updated ? Results.NoContent() : Results.NotFound();
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new {error = ex.Message});
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new {error = ex.Message});
                }
            });

        return group;
    }
}
