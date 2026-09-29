using WorkoutTracker.Core.DTOs.WorkoutExercise;

namespace WorkoutTracker.Services.Interfaces;

public interface IWorkoutExerciseService
{
    Task<List<WorkoutExerciseDto>> GetWorkoutExercisesAsync(string userId, Guid workoutId);
    Task<WorkoutExerciseDto?> CreateWorkoutExerciseAsync(string userId, Guid workoutId, CreateWorkoutExerciseDto dto);
    Task<bool> UpdateWorkoutExerciseAsync(string userId, Guid workoutId, Guid workoutExerciseId, UpdateWorkoutExerciseDto dto);
    Task<bool> DeleteWorkoutExerciseAsync(string userId, Guid workoutId, Guid workoutExerciseId);
}