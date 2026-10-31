using System.Net;
using System.Net.Http.Json;
using WorkoutTracker.Core.DTOs.Exercise;
using WorkoutTracker.Core.DTOs.Register;
using WorkoutTracker.Core.DTOs.Set;
using WorkoutTracker.Core.DTOs.Workout;
using WorkoutTracker.Core.DTOs.WorkoutExercise;

namespace WorkoutTracker.Prober;

public static class Program
{
    public static async Task Main()
    {
        var cookies = new CookieContainer();
        using var handler = new HttpClientHandler
        {
            CookieContainer = cookies,
            UseCookies = true,
            UseProxy = false
        };
        using var client = new HttpClient(handler);
        var baseUri = new Uri("http://localhost:8081");

        var password = "Qwerty1!";
        var email = $"probe-{Guid.NewGuid():N}@example.com";
        Console.WriteLine(email);
        var registerRequest = new RegisterDto("Probe User", email, password, password);
        var registerResponse = await client.PostAsJsonAsync($"{baseUri}api/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync($"{baseUri}login?useCookies=true",
            new
            {
                email,
                password
            });
        loginResponse.EnsureSuccessStatusCode();

        for (var i = 0; i < 10; i++)
        {
            var workoutRequest = new CreateWorkoutDto(DateOnly.FromDateTime(DateTime.UtcNow));
            var workoutResponse = await client.PostAsJsonAsync($"{baseUri}api/workouts", workoutRequest);
            workoutResponse.EnsureSuccessStatusCode();

            var workout = await workoutResponse.Content.ReadFromJsonAsync<WorkoutDto>();
            var workoutId = workout?.workoutId;

            var exerciseRequest = new CreateExerciseDto { Name = $"push-ups-{i}" };
            var exerciseResponse =
                await client.PostAsJsonAsync($"{baseUri}api/exercises", exerciseRequest);
            exerciseResponse.EnsureSuccessStatusCode();
            
            var exercise = await exerciseResponse.Content.ReadFromJsonAsync<ExerciseDto>();

            var workoutExerciseRequest = new CreateWorkoutExerciseDto((Guid)exercise?.Id);
            var workoutExerciseResponse = await client.PostAsJsonAsync($"{baseUri}api/workouts/{workoutId}/exercises",
                workoutExerciseRequest);
            workoutExerciseResponse.EnsureSuccessStatusCode();
            var workoutExercise = await workoutExerciseResponse.Content.ReadFromJsonAsync<WorkoutExerciseDto>();
            var workoutExerciseId = workoutExercise?.Id;
            if (i == 0)
            {
                var duplicateExerciseResponse =
                    await client.PostAsJsonAsync($"{baseUri}api/workouts/{workoutId}/exercises",
                        workoutExerciseRequest);
                Console.WriteLine($"Duplicate exercise: {duplicateExerciseResponse.StatusCode}");
            }

            var setRequest = new CreateSetDto(12, 12, 1);
            var setResponse =
                await client.PostAsJsonAsync($"{baseUri}api/workouts/{workoutId}/exercises/{workoutExerciseId}/sets",
                    setRequest);
            setResponse.EnsureSuccessStatusCode();
            if (i == 0)
            {
                var duplicateSetResponse =
                    await client.PostAsJsonAsync(
                        $"{baseUri}api/workouts/{workoutId}/exercises/{workoutExerciseId}/sets", setRequest);
                Console.WriteLine($"Duplicate set: {duplicateSetResponse.StatusCode}"); //409
            }
        }
    }
}