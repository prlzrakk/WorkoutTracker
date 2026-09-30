using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Core.DTOs.WorkoutExercise;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Services;

public class WorkoutExerciseService(ProjectContext context) : IWorkoutExerciseService
{
    public async Task<List<WorkoutExerciseDto>> GetWorkoutExercisesAsync(string userId, Guid workoutId)
    {
        return await context.WorkoutExercises
            .Where(w => w.WorkoutId == workoutId && w.Workout.UserId == userId)
            .Select(we => new WorkoutExerciseDto(we.Id, we.ExerciseId, we.WorkoutId))
            .ToListAsync();
    }

    public async Task<WorkoutExerciseDto?> CreateWorkoutExerciseAsync(string userId, Guid workoutId, CreateWorkoutExerciseDto dto)
    {
        var workoutExists = await context.Workouts.AnyAsync(w =>
            w.Id == workoutId && w.UserId == userId);
        if (!workoutExists)
            return null;
        var exercise = await context.Exercises
            .FirstOrDefaultAsync(e => e.Id == dto.ExerciseId && e.UserId == userId);
        if (exercise is null)
            return null;
        var exists = await context.WorkoutExercises.AnyAsync(we =>
            we.WorkoutId == workoutId &&
            we.ExerciseId == dto.ExerciseId);
        if (exists)
            throw new InvalidOperationException("Такое упражнение в этой тренировке уже существует");
        var workoutExercise = new WorkoutExercise
        {
            Id = Guid.NewGuid(),
            WorkoutId = workoutId,
            ExerciseId = dto.ExerciseId,
        };
        await context.WorkoutExercises.AddAsync(workoutExercise);
        await context.SaveChangesAsync();

        var result = new WorkoutExerciseDto(workoutExercise.Id, dto.ExerciseId, workoutId);
        return result;
    }

    public async Task<bool> UpdateWorkoutExerciseAsync(string userId, Guid workoutId, Guid workoutExerciseId, UpdateWorkoutExerciseDto dto)
    {
        var workoutExercise = await context.WorkoutExercises
            .FirstOrDefaultAsync(we => we.Id == workoutExerciseId && we.WorkoutId == workoutId && we.Workout.UserId == userId);
        if (workoutExercise is null)
            return false;
        var exercise = await context.Exercises
            .FirstOrDefaultAsync(e => e.Id == dto.ExerciseId && e.UserId == userId);
        if (exercise is null)
            return false;
        var exists = await context.WorkoutExercises.AnyAsync(we =>
            we.WorkoutId == workoutId &&
            we.ExerciseId == dto.ExerciseId &&
            we.Id != workoutExerciseId);
        if (exists)
            throw new InvalidOperationException("Такое упражнение в этой тренировке уже существует");
        workoutExercise.ExerciseId = dto.ExerciseId;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteWorkoutExerciseAsync(string userId, Guid workoutId, Guid workoutExerciseId)
    {
        var workoutExercise = await context.WorkoutExercises
            .FirstOrDefaultAsync(we => we.Id == workoutExerciseId && we.WorkoutId == workoutId && we.Workout.UserId == userId);
        if (workoutExercise is null)
            return false;
        context.WorkoutExercises.Remove(workoutExercise);
        await context.SaveChangesAsync();
        return true;
    }
}