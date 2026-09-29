using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WorkoutTracker.Client;
using MudBlazor.Services;
using WorkoutTracker.Client.Identity;
using WorkoutTracker.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var clientBaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
var apiBaseAddress = clientBaseAddress.Scheme == Uri.UriSchemeHttps
    ? new Uri("https://localhost:7294/")
    : new Uri("http://localhost:5149/");

builder.Services.AddScoped<WorkoutDraftSaver>();
builder.Services.AddMudServices();

builder.Services.AddTransient<CookieHandler>();
builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = apiBaseAddress;
})
.AddHttpMessageHandler<CookieHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>()
        .CreateClient("Api"));

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<CookieAuthenticationStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CookieAuthenticationStateProvider>());

await builder.Build().RunAsync();
