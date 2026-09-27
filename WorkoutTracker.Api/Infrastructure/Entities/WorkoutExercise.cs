namespace Infrastructure.Entities;

public class WorkoutExercise
{
    public Guid Id { get; set; }
    public Guid WorkoutId { get; set; }
    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; }
    public Workout Workout { get; set; }
    public ICollection<Set> Sets { get; set; } = [];
}