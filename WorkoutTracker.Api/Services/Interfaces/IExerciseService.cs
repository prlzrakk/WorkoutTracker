using Infrastructure.Entities;
using WorkoutTracker.Core.DTOs.Exercise;

namespace WorkoutTracker.Services.Interfaces;

public interface IExerciseService
{
    Task<List<ExerciseDto>> GetExercisesAsync();
    Task<ExerciseDto?> GetExerciseByIdAsync(Guid id);
    Task<ExerciseDto> CreateExerciseAsync(CreateExerciseDto createExerciseDto);
    Task<bool> UpdateExerciseAsync(Guid id, UpdateExerciseDto updateExerciseDto);
    Task<bool> DeleteExerciseAsync(Guid id);
}