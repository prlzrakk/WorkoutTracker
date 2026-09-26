using WorkoutTracker.Core.DTOs.Set;

namespace WorkoutTracker.Services.Interfaces;

public interface ISetService
{
    public Task<SetDto?> CreateSetAsync(Guid workoutId, Guid workoutExerciseId, CreateSetDto createSetDto);
    public Task<SetDto?> GetSetByIdAsync(Guid workoutId, Guid workoutExerciseId, Guid id);
    public Task<bool> DeleteSetAsync(Guid workoutId, Guid workoutExerciseId, Guid id);
    public Task<bool> UpdateSetAsync(Guid id,  Guid workoutId, Guid workoutExerciseId, UpdateSetDto updateSetDto);
}