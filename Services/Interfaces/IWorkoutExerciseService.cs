using WorkoutTracker.Core.DTOs.WorkoutExercise;

namespace WorkoutTracker.Services.Interfaces;

public interface IWorkoutExerciseService
{
    Task<List<WorkoutExerciseDto>> GetWorkoutExercisesAsync(Guid workoutId);
    Task<WorkoutExerciseDto?> CreateWorkoutExerciseAsync(Guid workoutId, CreateWorkoutExerciseDto dto);
    Task<bool> UpdateWorkoutExerciseAsync(Guid workoutId, Guid workoutExerciseId, UpdateWorkoutExerciseDto dto);
    Task<bool> DeleteWorkoutExerciseAsync(Guid workoutId, Guid workoutExerciseId);
}