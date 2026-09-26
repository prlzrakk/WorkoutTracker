using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Core.DTOs.Exercise;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Services;

public class ExerciseService(ProjectContext context) : IExerciseService
{
    public async Task<List<ExerciseDto>> GetExercisesAsync()
    {
        return await context.Exercises
            .Select(e => new ExerciseDto
            (
                e.Id,
                e.Name,
                e.MuscleGroup
            ))
            .ToListAsync();
    }

    public async Task<ExerciseDto?> GetExerciseByIdAsync(Guid id)
    {
        return await context.Exercises
            .Where(e => e.Id == id)
            .Select(e => new ExerciseDto(e.Id, e.Name, e.MuscleGroup))
            .FirstOrDefaultAsync();
    }

    public async Task<ExerciseDto> CreateExerciseAsync(CreateExerciseDto createExerciseDto)
    {
        if (string.IsNullOrWhiteSpace(createExerciseDto.Name) ||
            string.IsNullOrWhiteSpace(createExerciseDto.MuscleGroup))
            throw new ArgumentException("Поле не может быть пустым");

        var name = createExerciseDto.Name.Trim();
        var muscleGroup = createExerciseDto.MuscleGroup.Trim();
        var exists = await context.Exercises.AnyAsync(e => e.Name.ToLower() == name.ToLower());
        if (exists)
            throw new InvalidOperationException("Такое упражнение уже существует");

        var exercise = new Exercise
        {
            Name = name,
            MuscleGroup = muscleGroup,
        };

        await context.Exercises.AddAsync(exercise);
        await context.SaveChangesAsync();

        var result = new ExerciseDto
        (
            exercise.Id,
            exercise.Name,
            exercise.MuscleGroup
        );
        return result;
    }

    public async Task<bool> UpdateExerciseAsync(Guid id, UpdateExerciseDto updateExerciseDto)
    {
        var exercise = await context.Exercises.FindAsync(id);
        if (exercise == null)
            return false;

        if (string.IsNullOrWhiteSpace(updateExerciseDto.Name) ||
            string.IsNullOrWhiteSpace(updateExerciseDto.MuscleGroup))
            throw new ArgumentException("Поле не может быть пустым");

        var name = updateExerciseDto.Name.Trim();
        var muscleGroup = updateExerciseDto.MuscleGroup.Trim();

        var exists = await context.Exercises.AnyAsync(e =>
            e.Name.ToLower() == name.ToLower() &&
            e.Id != id);

        if (exists)
            throw new InvalidOperationException("Такое упражнение уже существует");

        exercise.Name = name;
        exercise.MuscleGroup = muscleGroup;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteExerciseAsync(Guid id)
    {
        var exercise = await context.Exercises.FindAsync(id);
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