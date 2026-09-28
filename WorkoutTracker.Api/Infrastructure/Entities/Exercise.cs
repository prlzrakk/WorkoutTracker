namespace Infrastructure.Entities;

public class Exercise
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = [];
}
