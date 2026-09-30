using Infrastructure.Db;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Core.DTOs.Workout;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Services;

public class WorkoutService(ProjectContext context, ILogger<WorkoutService> logger) : IWorkoutService
{
    public async Task<List<WorkoutDto>> GetWorkoutsAsync(string userId)
    {
        return await context.Workouts
            .Where(w => w.UserId == userId)
            .Select(w => new WorkoutDto(
                w.Id,
                w.Date,
                w.Note
            ))
            .ToListAsync();
    }

    public async Task<WorkoutDto?> GetWorkoutByIdAsync(string userId, Guid workoutId)
    {
        return await context.Workouts
            .Where(w => w.Id == workoutId && w.UserId == userId)
            .Select(w => new WorkoutDto(
                w.Id,
                w.Date,
                w.Note
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<WorkoutDto> CreateWorkoutAsync(string userId, CreateWorkoutDto createWorkoutDto)
    {
        var workout = new Workout
        {
            Date = createWorkoutDto.Date,
            UserId = userId
        };
        await context.Workouts.AddAsync(workout);
        await context.SaveChangesAsync();

        logger.LogInformation(
            "Workout created. WorkoutId={WorkoutId}, UserId={UserId}, Date={Date}",
            workout.Id,
            workout.UserId,
            workout.Date);

        return new WorkoutDto(
            workout.Id,
            workout.Date,
            workout.Note
        );
    }

    public async Task<bool> UpdateWorkoutAsync(string userId, Guid workoutId, UpdateWorkoutDto updateWorkoutDto)
    {
        var workout = await context.Workouts
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);
        if (workout == null)
        {
            logger.LogWarning("Workout update failed. WorkoutId={WorkoutId}, UserId={UserId}, Reason=NotFound", workoutId, userId);
            return false;
        }

        workout.Note = updateWorkoutDto.Note;
        await context.SaveChangesAsync();

        logger.LogInformation("Workout updated. WorkoutId={WorkoutId}, UserId={UserId}", workoutId, userId);
        return true;
    }

    public async Task<bool> DeleteWorkoutAsync(string userId, Guid workoutId)
    {
        var workout = await context.Workouts
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);
        if (workout == null)
        {
            logger.LogWarning("Workout delete failed. WorkoutId={WorkoutId}, UserId={UserId}, Reason=NotFound", workoutId, userId);
            return false;
        }

        context.Workouts.Remove(workout);
        await context.SaveChangesAsync();

        logger.LogInformation("Workout deleted. WorkoutId={WorkoutId}, UserId={UserId}", workoutId, userId);
        return true;
    }
}
