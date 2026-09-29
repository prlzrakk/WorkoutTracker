using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Core.DTOs.Set;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Services;

public class SetService(ProjectContext context) : ISetService
{
    public async Task<SetDto?> CreateSetAsync(string userId, Guid workoutId, Guid workoutExerciseId, CreateSetDto createSetDto)
    {
        var workoutExercise = await context.WorkoutExercises
            .FirstOrDefaultAsync(we => we.Id == workoutExerciseId &&
                                       we.WorkoutId == workoutId &&
                                       we.Workout.UserId == userId);
        if (workoutExercise == null)
            return null;
        if (createSetDto.Weight < 0 || createSetDto.Reps < 0 || createSetDto.SetNumber <= 0)
            throw new ArgumentException("Номер подхода, вес и количество повторений не могут быть отрицательными");
        var setNumberExists = await context.Sets.AnyAsync(s =>
                    s.WorkoutExerciseId == workoutExerciseId &&
                    s.SetNumber == createSetDto.SetNumber);
        if (setNumberExists)
            throw new InvalidOperationException("Такой номер подхода уже использовался");
        var set = new Set
        {
            Id = Guid.NewGuid(),
            Weight = createSetDto.Weight,
            Reps = createSetDto.Reps,
            SetNumber = createSetDto.SetNumber,
            WorkoutExerciseId = workoutExerciseId,
        };
        await context.Sets.AddAsync(set);
        await context.SaveChangesAsync();
        var result = new SetDto(set.Id, set.Weight, set.Reps, set.SetNumber);
        return result;
    }
    
    public async Task<List<SetDto>> GetSetsAsync(string userId, Guid workoutId, Guid workoutExerciseId)
    {
        return await context.Sets
            .Where(s => s.WorkoutExerciseId == workoutExerciseId &&
                        s.WorkoutExercise.WorkoutId == workoutId &&
                        s.WorkoutExercise.Workout.UserId == userId)
            .OrderBy(s => s.SetNumber)
            .Select(s => new SetDto(s.Id, s.Weight, s.Reps, s.SetNumber))
            .ToListAsync();
    }

    public async Task<SetDto?> GetSetByIdAsync(string userId, Guid workoutId, Guid workoutExerciseId, Guid id)
    {
        return await context.Sets
            .Where(s => s.Id == id && 
                        s.WorkoutExerciseId == workoutExerciseId && 
                        s.WorkoutExercise.WorkoutId == workoutId &&
                        s.WorkoutExercise.Workout.UserId == userId)
            .Select(s => new SetDto(s.Id, s.Weight, s.Reps, s.SetNumber))
            .FirstOrDefaultAsync();
    }
    
    public async Task<bool> DeleteSetAsync(string userId, Guid workoutId, Guid workoutExerciseId, Guid id)
    {
        var set = await context.Sets
            .FirstOrDefaultAsync(s =>
                s.Id == id &&
                s.WorkoutExerciseId == workoutExerciseId &&
                s.WorkoutExercise.WorkoutId == workoutId &&
                s.WorkoutExercise.Workout.UserId == userId);
        if (set == null)
            return false;
        context.Sets.Remove(set);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateSetAsync(string userId, Guid id,  Guid workoutId, Guid workoutExerciseId, UpdateSetDto updateSetDto)
    {
        var set = await context.Sets
            .FirstOrDefaultAsync(s => 
                s.Id == id && 
                s.WorkoutExerciseId == workoutExerciseId && 
                s.WorkoutExercise.WorkoutId == workoutId &&
                s.WorkoutExercise.Workout.UserId == userId);
        if (set == null)
            return false;
        var setNumberExists = await context.Sets.AnyAsync(s =>
            s.WorkoutExerciseId == workoutExerciseId &&
            s.SetNumber == updateSetDto.SetNumber &&
            s.Id != id);
        if (setNumberExists)
            throw new InvalidOperationException("Такой номер подхода уже использовался");
        if (updateSetDto.Weight < 0 || updateSetDto.Reps < 0 || updateSetDto.SetNumber <= 0)
            throw new ArgumentException("Номер подхода, вес и количество повторений не могут быть отрицательными");
        set.Weight = updateSetDto.Weight;
        set.Reps = updateSetDto.Reps;
        set.SetNumber = updateSetDto.SetNumber;
        await context.SaveChangesAsync();
        return true;
    }
}
