namespace WorkoutTracker.backend.DTOs.Exercise;

public class UpdateExerciseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string MuscleGroup { get; set; }
}