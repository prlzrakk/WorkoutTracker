namespace WorkoutTracker.Core.DTOs.Register;

public record RegisterDto(string Name, string Email, string Password, string PasswordConfirmation);