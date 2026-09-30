namespace Infrastructure.Entities;

public class Set
{
    public Guid Id { get; set; }
    public Guid WorkoutExerciseId { get; set; }
    public WorkoutExercise WorkoutExercise { get; set; }
    public double Weight { get; set; }
    public int Reps { get; set; }
    public int SetNumber { get; set; }
}