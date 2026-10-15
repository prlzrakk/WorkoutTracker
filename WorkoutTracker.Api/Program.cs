using WorkoutTracker.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddJsonLogging()
    .LoadEnvFiles()
    .AddClientCors()
    .AddProjectDatabase()
    .AddApplicationServices()
    .AddIdentityAuth();

var app = builder.Build();

app
    .UseApiMiddleware()
    .MapApiDocumentation()
    .MapEndpoints()
    .Run();