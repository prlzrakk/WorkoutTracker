using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WorkoutTracker.Client.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder
    .AddRootComponents()
    .AddClientServices()
    .AddApiClient()
    .AddClientAuth();

await builder.Build().RunAsync();