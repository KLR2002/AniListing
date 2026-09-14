using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AniListingFront;
using AniListingFront.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Authorization Core
builder.Services.AddAuthorizationCore();

// Auth State Provider
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());

// Backend API HttpClient
builder.Services.AddScoped(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var backendUrl = config["BackendUrl"];

    var baseUri = !string.IsNullOrWhiteSpace(backendUrl)
        ? new Uri(backendUrl)
        : new Uri(builder.HostEnvironment.BaseAddress);

    return new HttpClient { BaseAddress = baseUri };
});

// Api Client Service
builder.Services.AddScoped<IApiClient, ApiClient>();

await builder.Build().RunAsync();
