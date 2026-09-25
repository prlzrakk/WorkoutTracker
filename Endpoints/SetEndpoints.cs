using Infrastructure.Db;
using Infrastructure.Entities;
using WorkoutTracker.Core.DTOs.Set;

namespace WorkoutTracker.Api.Endpoints;

public static class SetEndpoints
{
    public static RouteGroupBuilder MapSets(this RouteGroupBuilder group)
    {
        group.MapGet("/{setId:guid}", async (ProjectContext context, Guid setId) =>
        {
            var set = await context.Sets.FindAsync(setId);
            if (set == null)
                return Results.NotFound();
            var result = new SetDto(
                set.Id,
                set.Weight,
                set.Reps,
                set.SetNumber
            );
            return Results.Ok(result);
        });

        group.MapPost("/",
            async (ProjectContext context, CreateSetDto newSet, Guid workoutId, Guid workoutExerciseId) =>
            {
                var workoutExercise = await context.WorkoutExercises.FindAsync(workoutExerciseId);
                if (workoutExercise == null)
                    return Results.NotFound();
                if (workoutExercise.WorkoutId != workoutId)
                    return Results.BadRequest();
                var set = new Set
                {
                    Id = Guid.NewGuid(),
                    Weight = newSet.Weight,
                    Reps = newSet.Reps,
                    SetNumber = newSet.SetNumber,
                    WorkoutExerciseId = workoutExerciseId,
                };
                await context.Sets.AddAsync(set);
                await context.SaveChangesAsync();
                var result = new SetDto(set.Id, set.Weight, set.Reps, set.SetNumber);
                return Results.Created($"/api/workouts/{workoutId}/exercises/{workoutExerciseId}/sets/{set.Id}",
                    result);
            });

        group.MapDelete("/{setId:guid}", async (ProjectContext context, Guid setId) =>
        {
            var set = await context.Sets.FindAsync(setId);
            if (set == null)
                return Results.NotFound();
            context.Sets.Remove(set);
            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapPatch("/{setId:guid}", async (ProjectContext context, Guid setId, UpdateSetDto updatedSet) =>
        {
            var set = await context.Sets.FindAsync(setId);
            if (set == null)
                return Results.NotFound();
            set.Weight = updatedSet.Weight;
            set.Reps = updatedSet.Reps;
            set.SetNumber = updatedSet.SetNumber;
            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        return group;
    }
}