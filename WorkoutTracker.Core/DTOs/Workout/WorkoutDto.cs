namespace WorkoutTracker.Core.DTOs.Workout;

public record WorkoutDto(Guid workoutId, DateOnly Date, string? Note);
