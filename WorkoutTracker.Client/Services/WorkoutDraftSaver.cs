using System.Net.Http.Json;
using WorkoutTracker.Client.Models;
using WorkoutTracker.Core.DTOs.Exercise;
using WorkoutTracker.Core.DTOs.Set;
using WorkoutTracker.Core.DTOs.WorkoutExercise;

namespace WorkoutTracker.Client.Services;

public class WorkoutDraftSaver(HttpClient http)
{
    public async Task<SaveExercisesResult> SaveExercisesToWorkoutAsync(
        Guid workoutId,
        List<DraftExercise> draftExercises,
        List<ExerciseDto> availableExercises)
    {
        List<SavedWorkoutExercise> savedExercises = [];

        foreach (var draftExercise in draftExercises)
        {
            var exerciseName = draftExercise.SearchText.Trim();
            if (string.IsNullOrWhiteSpace(exerciseName))
                continue;

            var exercise = await ResolveExerciseAsync(draftExercise, exerciseName, availableExercises);
            if (exercise is null)
                return SaveExercisesResult.Fail($"Exercise \"{exerciseName}\" was not saved");

            var workoutExerciseResponse = await http.PostAsJsonAsync(
                $"api/workouts/{workoutId}/exercises",
                new CreateWorkoutExerciseDto(exercise.Id));

            if (!workoutExerciseResponse.IsSuccessStatusCode)
                return SaveExercisesResult.Fail($"Exercise \"{exercise.Name}\" was not added to workout");

            var workoutExercise = await workoutExerciseResponse.Content.ReadFromJsonAsync<WorkoutExerciseDto>();
            if (workoutExercise is null)
                return SaveExercisesResult.Fail($"Exercise \"{exercise.Name}\" was not added to workout");

            var savedSets = await SaveSetsAsync(workoutId, workoutExercise.Id, exercise.Name, draftExercise.Sets);
            if (!savedSets.Success)
                return SaveExercisesResult.Fail(savedSets.ErrorMessage);

            savedExercises.Add(new SavedWorkoutExercise(
                workoutExercise.Id,
                exercise.Name,
                savedSets.Sets));
        }

        if (savedExercises.Count == 0)
            return SaveExercisesResult.Fail("Add at least one exercise");

        return SaveExercisesResult.Ok(savedExercises);
    }

    private async Task<ExerciseDto?> ResolveExerciseAsync(
        DraftExercise draftExercise,
        string exerciseName,
        List<ExerciseDto> availableExercises)
    {
        var exerciseId = draftExercise.ExerciseId ?? FindExerciseIdByName(exerciseName, availableExercises);
        if (exerciseId is not null)
        {
            var existingExercise = availableExercises.FirstOrDefault(exercise => exercise.Id == exerciseId.Value);
            if (existingExercise is not null)
                return existingExercise;
        }

        var createdExercise = await CreateExerciseAsync(exerciseName);
        if (createdExercise is null)
            return null;

        availableExercises.Add(createdExercise);
        draftExercise.ExerciseId = createdExercise.Id;
        draftExercise.SearchText = createdExercise.Name;

        return createdExercise;
    }

    private static Guid? FindExerciseIdByName(string name, List<ExerciseDto> availableExercises)
    {
        var trimmedName = name.Trim();
        if (string.IsNullOrWhiteSpace(trimmedName))
            return null;

        return availableExercises.FirstOrDefault(exercise =>
            string.Equals(exercise.Name, trimmedName, StringComparison.OrdinalIgnoreCase))?.Id;
    }

    private async Task<ExerciseDto?> CreateExerciseAsync(string name)
    {
        var trimmedName = name.Trim();
        if (string.IsNullOrWhiteSpace(trimmedName))
            return null;

        var response = await http.PostAsJsonAsync(
            "api/exercises",
            new CreateExerciseDto { Name = trimmedName });

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ExerciseDto>();
    }

    private async Task<SaveSetsResult> SaveSetsAsync(
        Guid workoutId,
        Guid workoutExerciseId,
        string exerciseName,
        List<DraftSet> sets)
    {
        List<SetDto> savedSets = [];

        foreach (var set in sets)
        {
            if (set.Reps is null || set.Weight is null)
                continue;

            var setResponse = await http.PostAsJsonAsync(
                $"api/workouts/{workoutId}/exercises/{workoutExerciseId}/sets",
                new CreateSetDto(set.Weight.Value, set.Reps.Value, set.Number));

            if (!setResponse.IsSuccessStatusCode)
                return SaveSetsResult.Fail($"Set {set.Number} for \"{exerciseName}\" was not saved");

            var savedSet = await setResponse.Content.ReadFromJsonAsync<SetDto>();
            if (savedSet is not null)
                savedSets.Add(savedSet);
        }

        return SaveSetsResult.Ok(savedSets.OrderBy(set => set.SetNumber).ToList());
    }
}

public sealed record SavedWorkoutExercise(Guid WorkoutExerciseId, string Name, List<SetDto> Sets);

public sealed record SaveExercisesResult(bool Success, string? ErrorMessage, List<SavedWorkoutExercise> Exercises)
{
    public static SaveExercisesResult Ok(List<SavedWorkoutExercise> exercises)
    {
        return new SaveExercisesResult(true, null, exercises);
    }

    public static SaveExercisesResult Fail(string? errorMessage)
    {
        return new SaveExercisesResult(false, errorMessage, []);
    }
}

internal sealed record SaveSetsResult(bool Success, string? ErrorMessage, List<SetDto> Sets)
{
    public static SaveSetsResult Ok(List<SetDto> sets)
    {
        return new SaveSetsResult(true, null, sets);
    }

    public static SaveSetsResult Fail(string? errorMessage)
    {
        return new SaveSetsResult(false, errorMessage, []);
    }
}