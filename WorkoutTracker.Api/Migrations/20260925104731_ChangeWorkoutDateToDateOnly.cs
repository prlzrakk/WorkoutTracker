using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutTracker.Migrations
{
    /// <inheritdoc />
    public partial class ChangeWorkoutDateToDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Workouts");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Workouts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "Workouts",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "Sets",
                type: "double precision",
                precision: 6,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(6,2)",
                oldPrecision: 6,
                oldScale: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Workouts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Workouts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "Workouts",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "Sets",
                type: "numeric(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 6,
                oldScale: 2);
        }
    }
}
