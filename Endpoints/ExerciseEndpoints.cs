using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Core.DTOs.Exercise;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Api.Endpoints;

public static class ExerciseEndpoints
{
    public static RouteGroupBuilder MapExerciseEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IExerciseService service) =>
        {
            var exercises = await service.GetExercisesAsync();
            return Results.Ok(exercises);
        });

        group.MapGet("/{id:guid}", async (IExerciseService service, Guid id) =>
        {
            var exercise = await service.GetExerciseByIdAsync(id);
            return exercise is null ? Results.NotFound() : Results.Ok(exercise);
        });

        group.MapPatch("/{id:guid}", async (IExerciseService service, Guid id, UpdateExerciseDto updatedExercise) =>
        {
            try
            {
                var updated = await service.UpdateExerciseAsync(id, updatedExercise);
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

        group.MapDelete("/{id:guid}", async (IExerciseService service, Guid id) =>
        {
            try
            {
                var deleted = await service.DeleteExerciseAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new {error = ex.Message});
            }
        });

        group.MapPost("/", async (IExerciseService service, CreateExerciseDto createdExerciseDto) =>
        {
            try
            {
                var result = await service.CreateExerciseAsync(createdExerciseDto);
                return Results.Created($"api/exercises/{result.Id}", result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new {error = ex.Message});
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new {error = ex.Message});
            }
        });

        return group;
    }
}