using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using WorkoutTracker.Client.Identity;
using WorkoutTracker.Client.Services;

namespace WorkoutTracker.Client.Extensions;

public static class ClientExtensions
{
    public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        return builder;
    }
    
    public static WebAssemblyHostBuilder AddApiClient(this WebAssemblyHostBuilder builder)
    {
        var clientBaseAddress = new Uri(builder.HostEnvironment.BaseAddress);

        var apiBaseUrl = clientBaseAddress.Scheme == Uri.UriSchemeHttps
            ? builder.Configuration["Api:HttpsBaseUrl"]
            : builder.Configuration["Api:HttpBaseUrl"];

        if (string.IsNullOrWhiteSpace(apiBaseUrl))
            throw new InvalidOperationException("Api base url is not configured");

        var apiBaseAddress = new Uri(clientBaseAddress, apiBaseUrl);

        builder.Services.AddTransient<CookieHandler>();

        builder.Services.AddHttpClient("Api", client =>
            {
                client.BaseAddress = apiBaseAddress;
            })
            .AddHttpMessageHandler<CookieHandler>();

        builder.Services.AddScoped(sp =>
            sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient("Api"));

        return builder;
    }
    
    public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<WorkoutDraftSaver>();
        builder.Services.AddMudServices();

        return builder;
    }

    public static WebAssemblyHostBuilder AddClientAuth(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddAuthorizationCore();

        builder.Services.AddScoped<CookieAuthenticationStateProvider>();

        builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
            sp.GetRequiredService<CookieAuthenticationStateProvider>());

        return builder;
    }
    
}
