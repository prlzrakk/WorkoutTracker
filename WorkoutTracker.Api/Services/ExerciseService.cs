using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Core.DTOs.Exercise;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Services;

public class ExerciseService(ProjectContext context, ILogger<ExerciseService> logger) : IExerciseService
{
    public async Task<List<ExerciseDto>> GetExercisesAsync(string userId)
    {
        return await context.Exercises
            .Where(e => e.UserId == userId)
            .Select(e => new ExerciseDto
            (
                e.Id,
                e.Name
            ))
            .ToListAsync();
    }

    public async Task<ExerciseDto?> GetExerciseByIdAsync(string userId, Guid id)
    {
        return await context.Exercises
            .Where(e => e.Id == id && e.UserId == userId)
            .Select(e => new ExerciseDto(e.Id, e.Name))
            .FirstOrDefaultAsync();
    }

    public async Task<ExerciseDto> CreateExerciseAsync(string userId, CreateExerciseDto createExerciseDto)
    {
        if (string.IsNullOrWhiteSpace(createExerciseDto.Name))
        {
            logger.LogWarning("Exercise creation failed. UserId={UserId}, Reason=NameRequired", userId);
            throw new ArgumentException("Поле не может быть пустым");
        }

        var name = createExerciseDto.Name.Trim();
        var exists = await context.Exercises.AnyAsync(e => e.Name.ToLower() == name.ToLower() && e.UserId == userId);
        if (exists)
        {
            logger.LogWarning(
                "Exercise creation failed. UserId={UserId}, ExerciseName={ExerciseName}, Reason=AlreadyExists",
                userId,
                name);

            throw new InvalidOperationException("Такое упражнение уже существует");
        }

        var exercise = new Exercise
        {
            Name = name,
            UserId = userId
        };

        await context.Exercises.AddAsync(exercise);
        await context.SaveChangesAsync();

        logger.LogInformation(
            "Exercise created. ExerciseId={ExerciseId}, UserId={UserId}, ExerciseName={ExerciseName}",
            exercise.Id,
            userId,
            exercise.Name);

        var result = new ExerciseDto
        (
            exercise.Id,
            exercise.Name
        );
        return result;
    }

    public async Task<bool> UpdateExerciseAsync(string userId, Guid id, UpdateExerciseDto updateExerciseDto)
    {
        var exercise = await context.Exercises
            .FirstOrDefaultAsync(e => e.UserId == userId && e.Id == id);
        if (exercise == null)
        {
            logger.LogWarning("Exercise update failed. ExerciseId={ExerciseId}, UserId={UserId}, Reason=NotFound", id, userId);
            return false;
        }


        if (string.IsNullOrWhiteSpace(updateExerciseDto.Name))
        {
            logger.LogWarning("Exercise update failed. ExerciseId={ExerciseId}, UserId={UserId}, Reason=NameRequired", id, userId);
            throw new ArgumentException("Поле не может быть пустым");
        }


        var name = updateExerciseDto.Name.Trim();

        var exists = await context.Exercises.AnyAsync(e =>
            e.Name.ToLower() == name.ToLower() &&
            e.Id != id &&
            e.UserId == userId);

        if (exists)
        {
            logger.LogWarning(
                "Exercise update failed. ExerciseId={ExerciseId}, UserId={UserId}, ExerciseName={ExerciseName}, Reason=AlreadyExists",
                id,
                userId,
                name);

            throw new InvalidOperationException("Такое упражнение уже существует");
        }

        exercise.Name = name;

        await context.SaveChangesAsync();
        logger.LogInformation(
            "Exercise updated. ExerciseId={ExerciseId}, UserId={UserId}, ExerciseName={ExerciseName}",
            exercise.Id,
            userId,
            exercise.Name);

        return true;
    }

    public async Task<bool> DeleteExerciseAsync(string userId, Guid id)
    {
        var exercise = await context.Exercises.FirstOrDefaultAsync(e => e.UserId == userId && e.Id == id);
        if (exercise == null)
        {
            logger.LogWarning("Exercise delete failed. ExerciseId={ExerciseId}, UserId={UserId}, Reason=NotFound", id, userId);
            return false;
        }

        var isUsed = await context.WorkoutExercises
            .AnyAsync(we => we.ExerciseId == id);
        if (isUsed)
        {
            logger.LogWarning("Exercise delete failed. ExerciseId={ExerciseId}, UserId={UserId}, Reason=InUse", id, userId);
            throw new InvalidOperationException(
                "Нельзя удалить упражнение, так как оно сейчас используется в ваших тренировках.");
        }

        context.Exercises.Remove(exercise);
        await context.SaveChangesAsync();

        logger.LogInformation(
            "Exercise deleted. ExerciseId={ExerciseId}, UserId={UserId}, ExerciseName={ExerciseName}",
            exercise.Id,
            userId,
            exercise.Name);

        return true;
    }
}
