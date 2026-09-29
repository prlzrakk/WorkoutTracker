using WorkoutTracker.Core.DTOs.Set;

namespace WorkoutTracker.Services.Interfaces;

public interface ISetService
{
    public Task<SetDto?> CreateSetAsync(string userId, Guid workoutId, Guid workoutExerciseId, CreateSetDto createSetDto);
    public Task<List<SetDto>> GetSetsAsync(string userId, Guid workoutId, Guid workoutExerciseId);
    public Task<SetDto?> GetSetByIdAsync(string userId, Guid workoutId, Guid workoutExerciseId, Guid id);
    public Task<bool> DeleteSetAsync(string userId, Guid workoutId, Guid workoutExerciseId, Guid id);
    public Task<bool> UpdateSetAsync(string userId, Guid id,  Guid workoutId, Guid workoutExerciseId, UpdateSetDto updateSetDto);
}
