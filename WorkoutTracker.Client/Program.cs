using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WorkoutTracker.Client;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var clientBaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
var apiBaseAddress = clientBaseAddress.Scheme == Uri.UriSchemeHttps
    ? new Uri("https://localhost:7294/")
    : new Uri("http://localhost:5149/");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = apiBaseAddress });
builder.Services.AddMudServices();

await builder.Build().RunAsync();
