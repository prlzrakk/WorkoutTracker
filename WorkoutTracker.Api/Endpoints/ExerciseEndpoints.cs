using System.Security.Claims;
using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Api.Extensions;
using WorkoutTracker.Core.DTOs.Exercise;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Api.Endpoints;

public static class ExerciseEndpoints
{
    public static RouteGroupBuilder MapExerciseEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (ClaimsPrincipal user, IExerciseService service) =>
        {
            var exercises = await service.GetExercisesAsync(user.GetUserId());
            return Results.Ok(exercises);
        });

        group.MapGet("/{id:guid}", async (ClaimsPrincipal user, IExerciseService service, Guid id) =>
        {
            var exercise = await service.GetExerciseByIdAsync(user.GetUserId(), id);
            return exercise is null ? Results.NotFound() : Results.Ok(exercise);
        });

        group.MapPatch("/{id:guid}", async (ClaimsPrincipal user, IExerciseService service, Guid id, UpdateExerciseDto updatedExercise) =>
        {
            try
            {
                var updated = await service.UpdateExerciseAsync(user.GetUserId(), id, updatedExercise);
                return updated ? Results.NoContent() : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapDelete("/{id:guid}", async (ClaimsPrincipal user, IExerciseService service, Guid id) =>
        {
            try
            {
                var deleted = await service.DeleteExerciseAsync(user.GetUserId(), id);
                return deleted ? Results.NoContent() : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
        });

        group.MapPost("/", async (ClaimsPrincipal user, IExerciseService service, CreateExerciseDto createdExerciseDto) =>
        {
            try
            {
                var result = await service.CreateExerciseAsync(user.GetUserId(), createdExerciseDto);
                return Results.Created($"api/exercises/{result.Id}", result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
        });

        return group;
    }
}