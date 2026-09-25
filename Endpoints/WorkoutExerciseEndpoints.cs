using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Core.DTOs.Exercise;
using WorkoutTracker.Core.DTOs.WorkoutExercise;

namespace WorkoutTracker.Api.Endpoints;

public static class WorkoutExerciseEndpoints
{
    public static RouteGroupBuilder MapWorkoutExercise(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (ProjectContext context, Guid workoutId) =>
        {
            var result = await context.WorkoutExercises
                .Where(w => w.WorkoutId == workoutId)
                .Select(we => new WorkoutExerciseDto(we.Id, we.ExerciseId, we.WorkoutId))
                .ToListAsync();
            return Results.Ok(result);
        });

        group.MapDelete("/{exerciseId:guid}", async (ProjectContext context, Guid workoutId, Guid exerciseId) =>
        {
            var workoutExercise = await context.WorkoutExercises
                .Where(w => w.WorkoutId == workoutId && w.ExerciseId == exerciseId)
                .FirstOrDefaultAsync();
            if (workoutExercise is null)
                return Results.NotFound();
            context.WorkoutExercises.Remove(workoutExercise);
            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapPost("/",
            async (ProjectContext context, CreateWorkoutExerciseDto dto, Guid workoutId) =>
            {
                var workout = await context.Workouts.FindAsync(workoutId);
                if (workout is null)
                    return Results.NotFound();
                var exercise = await context.Exercises.FindAsync(dto.ExerciseId);
                if (exercise is null)
                    return Results.NotFound();
                var workoutExercise = new WorkoutExercise
                {
                    Id = Guid.NewGuid(),
                    WorkoutId = workoutId,
                    ExerciseId = dto.ExerciseId,
                };
                await context.WorkoutExercises.AddAsync(workoutExercise);
                await context.SaveChangesAsync();

                var result = new WorkoutExerciseDto(workoutExercise.Id, dto.ExerciseId, workoutId);
                return Results.Created($"/api/workouts/{workoutId}/exercises/{workoutExercise.Id}", result);
            });

        group.MapPatch("/{workoutExerciseId:guid}", async (ProjectContext context, Guid workoutId,
            Guid workoutExerciseId, UpdateWorkoutExerciseDto updatedDto) =>
        {
            var workoutExercise = await context.WorkoutExercises
                .FirstOrDefaultAsync(we => we.Id == workoutExerciseId && we.WorkoutId == workoutId);
            if (workoutExercise is null)
                return Results.NotFound();
            var exercise = await context.Exercises.FindAsync(updatedDto.ExerciseId);
            if (exercise is null)
                return Results.NotFound();
            workoutExercise.ExerciseId = updatedDto.ExerciseId;
            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        return group;
    }
}