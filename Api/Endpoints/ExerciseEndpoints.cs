using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.backend.DTOs.Exercise;

namespace WorkoutTracker.Api.Endpoints;

public static class ExerciseEndpoints
{
    public static RouteGroupBuilder MapExerciseEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (ProjectContext context) =>
        {
            var result = await context.Exercises
                .Select(e => new ExerciseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    MuscleGroup = e.MuscleGroup,
                })
                .ToListAsync();
            return Results.Ok(result);
        });

        group.MapGet("/{id}", async (ProjectContext context, int id) =>
        {
            var exercise = await context.Exercises.FindAsync(id);
            if (exercise == null)
                return Results.NotFound();
            var result = new ExerciseDto
            {
                Id = exercise.Id,
                Name = exercise.Name,
                MuscleGroup = exercise.MuscleGroup,
            };
            return Results.Ok(result);
        });

        group.MapPatch("/{id}", async (ProjectContext context, int id, UpdateExerciseDto updatedExercise) =>
        {
            var exercise = await context.Exercises.FindAsync(id);
            if (exercise == null)
                return Results.NotFound();
            exercise.Name = updatedExercise.Name;
            exercise.MuscleGroup = updatedExercise.MuscleGroup;
            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (ProjectContext context, int id) =>
        {
            var exercise = await context.Exercises.FindAsync(id);
            if (exercise == null)
                return Results.NotFound();
            context.Exercises.Remove(exercise);
            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapPost("/", async (ProjectContext context, CreateExerciseDto addedExercise) =>
        {
            var exercise = new Exercise
            {
                Name = addedExercise.Name,
                MuscleGroup = addedExercise.MuscleGroup,
            };
            context.Exercises.Add(exercise);
            await context.SaveChangesAsync();
            var result = new ExerciseDto
            {
                Id = exercise.Id,
                Name = exercise.Name,
                MuscleGroup = exercise.MuscleGroup,
            };
            
            return Results.Created($"api/exercises/{exercise.Id}", result);
        });

        return group;
    }
}