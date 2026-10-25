using System.Diagnostics.Metrics;

namespace WorkoutTracker.Api.Metrics;

public class BusinessMetrics
{
    private readonly Counter<long> workoutsCreatedCount;
    private readonly Counter<long> registerSuccesCount;
    private readonly Histogram<long> exercisesPerWorkoutHistogram;
    private readonly Histogram<long> setsPerExerciseHistogram;

    public BusinessMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("WorkoutTracker.Business");
        workoutsCreatedCount = meter.CreateCounter<long>(
            name: "workouts_created_total",
            description: "Number of successfully created workouts");
        registerSuccesCount = meter.CreateCounter<long>(
            name: "registration_success_total",
            description: "Number of successful registrations");
        exercisesPerWorkoutHistogram = meter.CreateHistogram<long>(
            name: "exercises_per_workout",
            description: "Number of exercises per workout");
        setsPerExerciseHistogram = meter.CreateHistogram<long>(
            name: "sets_per_exercise",
            description: "Number of sets per exercise");
    }

    public void TrackCreatedWorkout()
    {
        workoutsCreatedCount.Add(1);
    }

    public void TrackRegistrationSuccess()
    {
        registerSuccesCount.Add(1);
    }
    
    public void TrackExercisesPerWorkout(int exerciseCount)
    {
        exercisesPerWorkoutHistogram.Record(exerciseCount);
    }

    public void TrackSetsPerExercise(int setCount)
    {
        setsPerExerciseHistogram.Record(setCount);
    }
}