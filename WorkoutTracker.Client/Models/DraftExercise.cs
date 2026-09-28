namespace WorkoutTracker.Client.Models;

public class DraftExercise
{
    public Guid? ExerciseId { get; set; }
    public string SearchText { get; set; } = string.Empty;
    public bool IsSearchOpen { get; set; }
    public List<DraftSet> Sets { get; } = [new() { Number = 1 }];
}
