using Infrastructure.Entities;
using WorkoutTracker.Core.DTOs.Exercise;

namespace WorkoutTracker.Services.Interfaces;

public interface IExerciseService
{
    Task<List<ExerciseDto>> GetExercisesAsync(string userId);
    Task<ExerciseDto?> GetExerciseByIdAsync(string userId, Guid id);
    Task<ExerciseDto> CreateExerciseAsync(string userId, CreateExerciseDto createExerciseDto);
    Task<bool> UpdateExerciseAsync(string userId, Guid id, UpdateExerciseDto updateExerciseDto);
    Task<bool> DeleteExerciseAsync(string userId, Guid id);
}