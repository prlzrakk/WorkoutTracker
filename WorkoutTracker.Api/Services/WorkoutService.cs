using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Core.DTOs.Workout;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Services;

public class WorkoutService(ProjectContext context) : IWorkoutService
{
    public async Task<List<WorkoutDto>> GetWorkoutsAsync()
    {
        return await context.Workouts
            .Select(w => new WorkoutDto(
                w.Id,
                w.Date,
                w.Note
            ))
            .ToListAsync();
    }

    public async Task<WorkoutDto?> GetWorkoutByIdAsync(Guid workoutId)
    {
        return await context.Workouts
            .Where(w => w.Id == workoutId)
            .Select(w => new WorkoutDto(
                w.Id,
                w.Date,
                w.Note
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutDto createWorkoutDto)
    {
        var workout = new Workout
        {
            Date = createWorkoutDto.Date
        };
        await context.Workouts.AddAsync(workout);
        await context.SaveChangesAsync();
        return new WorkoutDto(
            workout.Id,
            workout.Date,
            workout.Note
        );
    }

    public async Task<bool> UpdateWorkoutAsync(Guid workoutId, UpdateWorkoutDto updateWorkoutDto)
    {
        var workout = await context.Workouts.FindAsync(workoutId);
        if (workout == null)
            return false;
        workout.Note = updateWorkoutDto.Note;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteWorkoutAsync(Guid workoutId)
    {
        var workout = await context.Workouts.FindAsync(workoutId);
        if (workout == null)
            return false;
        context.Workouts.Remove(workout);
        await context.SaveChangesAsync();
        return true;
    }
}
