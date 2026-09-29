namespace Infrastructure.Entities;

public class Workout
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public string? Note { get; set; } = null!;
    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = [];
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
