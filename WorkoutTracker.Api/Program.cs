using WorkoutTracker.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder
    .LoadEnvFiles()
    .AddClientCors()
    .AddProjectDatabase()
    .AddApplicationServices()
    .AddIdentityAuth();

var app = builder.Build();
app.UseApiMiddleware();
app.MapApiDocumentation();

app.MapEndpoints();

app.Run();