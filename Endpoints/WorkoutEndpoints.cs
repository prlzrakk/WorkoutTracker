using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Core.DTOs.Workout;

namespace WorkoutTracker.Api.Endpoints;

public static class WorkoutEndpoints
{
    public static RouteGroupBuilder MapWorkouts(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (ProjectContext context) =>
        {
            var result = await context.Workouts
                .Select(w => new WorkoutDto(w.Id, w.Date, w.Note))
                .ToListAsync();
            return Results.Ok(result);
        });

        group.MapGet("/{workoutId:guid}", async (ProjectContext context, Guid workoutId) =>
        {
            var workout = await context.Workouts.FindAsync(workoutId);
            if (workout == null)
                return Results.NotFound();
            var workoutDto = new WorkoutDto(workout.Id, workout.Date, workout.Note);
            return Results.Ok(workoutDto);
        });

        group.MapDelete("/{workoutId:guid}", async (ProjectContext context, Guid workoutId) =>
        {
            var workout = await context.Workouts.FindAsync(workoutId);
            if (workout == null)
                return Results.NotFound();
            context.Workouts.Remove(workout);
            await context.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapPatch("/{workoutId:guid}",
            async (ProjectContext context, Guid workoutId, UpdateWorkoutDto updatedWorkout) =>
            {
                var workout = await context.Workouts.FindAsync(workoutId);
                if (workout == null)
                    return Results.NotFound();
                workout.Note = updatedWorkout.Note;
                await context.SaveChangesAsync();
                return Results.NoContent();
            });

        group.MapPost("/", async (ProjectContext context, CreateWorkoutDto createdWorkout) =>
        {
            var workout = new Workout()
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Note = createdWorkout.Note,
            };
            await context.Workouts.AddAsync(workout);
            await context.SaveChangesAsync();
            var workoutDto = new WorkoutDto(workout.Id, workout.Date, workout.Note);
            return Results.Created($"api/workouts/{workout.Id}", workoutDto);
        });


        return group;
    }
}