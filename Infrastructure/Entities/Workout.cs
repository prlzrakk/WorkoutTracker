namespace Infrastructure.Entities;

public class Workout
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
    public string Note { get; set; }
    public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
}