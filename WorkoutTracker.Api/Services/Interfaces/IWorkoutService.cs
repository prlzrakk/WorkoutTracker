using WorkoutTracker.Core.DTOs.Workout;

namespace WorkoutTracker.Services.Interfaces;

public interface IWorkoutService
{
    Task<List<WorkoutDto>> GetWorkoutsAsync(string userId);
    Task<WorkoutDto?> GetWorkoutByIdAsync(string userId, Guid workoutId);
    Task<WorkoutDto> CreateWorkoutAsync(string userId, CreateWorkoutDto createWorkoutDto);
    Task<bool> UpdateWorkoutAsync(string userId, Guid workoutId, UpdateWorkoutDto updateWorkoutDto);
    Task<bool> DeleteWorkoutAsync(string userId, Guid workoutId);
}