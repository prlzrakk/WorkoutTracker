using WorkoutTracker.Core.DTOs.Workout;

namespace WorkoutTracker.Services.Interfaces;

public interface IWorkoutService
{
    Task<List<WorkoutDto>> GetWorkoutsAsync();
    Task<WorkoutDto?> GetWorkoutByIdAsync(Guid workoutId);
    Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutDto createWorkoutDto);
    Task<bool> UpdateWorkoutAsync(Guid workoutId, UpdateWorkoutDto updateWorkoutDto);
    Task<bool> DeleteWorkoutAsync(Guid workoutId);
}