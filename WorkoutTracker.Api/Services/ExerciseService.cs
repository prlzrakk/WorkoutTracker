using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Core.DTOs.Exercise;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Services;

public class ExerciseService(ProjectContext context) : IExerciseService
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
            throw new ArgumentException("Поле не может быть пустым");

        var name = createExerciseDto.Name.Trim();
        var exists = await context.Exercises.AnyAsync(e => e.Name.ToLower() == name.ToLower() && e.UserId == userId);
        if (exists)
            throw new InvalidOperationException("Такое упражнение уже существует");

        var exercise = new Exercise
        {
            Name = name,
            UserId = userId
        };

        await context.Exercises.AddAsync(exercise);
        await context.SaveChangesAsync();

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
            return false;

        if (string.IsNullOrWhiteSpace(updateExerciseDto.Name))
            throw new ArgumentException("Поле не может быть пустым");

        var name = updateExerciseDto.Name.Trim();

        var exists = await context.Exercises.AnyAsync(e =>
            e.Name.ToLower() == name.ToLower() &&
            e.Id != id &&
            e.UserId == userId);

        if (exists)
            throw new InvalidOperationException("Такое упражнение уже существует");

        exercise.Name = name;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteExerciseAsync(string userId, Guid id)
    {
        var exercise = await context.Exercises.FirstOrDefaultAsync(e => e.UserId == userId && e.Id == id);
        if (exercise == null)
            return false;

        var isUsed = await context.WorkoutExercises
            .AnyAsync(we => we.ExerciseId == id);
        if (isUsed)
            throw new InvalidOperationException("Нельзя удалить упражнение, так как оно сейчас используется в ваших тренировках.");

        context.Exercises.Remove(exercise);
        await context.SaveChangesAsync();
        return true;
    }
}
