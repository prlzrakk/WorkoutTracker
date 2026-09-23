using Microsoft.EntityFrameworkCore;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Db;

public class ProjectContext : DbContext
{
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<Set> Sets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExerciseConfiguration());
        modelBuilder.ApplyConfiguration(new WorkoutExerciseConfiguration());
        modelBuilder.ApplyConfiguration(new SetConfiguration());
        modelBuilder.ApplyConfiguration(new WorkoutConfiguration());
    }
}

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.MuscleGroup).IsRequired();
        builder.HasMany(e => e.WorkoutExercises).WithOne(e => e.Exercise).HasForeignKey(e => e.ExerciseId);
    }
}

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Date).IsRequired();
        builder.Property(w => w.Duration).IsRequired();
        builder.Property(w => w.Note);
        builder.HasMany(w => w.WorkoutExercises).WithOne(we => we.Workout).HasForeignKey(we => we.WorkoutId);
    }
}

public class SetConfiguration : IEntityTypeConfiguration<Set>
{
    public void Configure(EntityTypeBuilder<Set> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Reps).IsRequired();
        builder.Property(s => s.Weight).IsRequired();
        builder.Property(s => s.SetNumber).IsRequired();
        builder.HasOne(s => s.WorkoutExercise).WithMany(we => we.Sets).HasForeignKey(s => s.WorkoutExerciseId);
    }
}

public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
    {
        builder.HasKey(we => we.Id);
        builder.Property(we => we.ExerciseId).IsRequired();
        builder.Property(we => we.WorkoutId).IsRequired();
    }
}