using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UserManagement.Blazor;
using UserManagement.Blazor.Providers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register a new AuthenticationStateProvider for JWT authentication
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Configure a custom handler to automatically add the JWT token to requests
builder.Services.AddScoped<TokenHandler>();

builder.Services.AddScoped<AuthenticationService>();

// Configure HttpClient to use the token handler
builder.Services.AddScoped(sp => new HttpClient(sp.GetRequiredService<TokenHandler>())
{
    BaseAddress = new Uri(builder.Configuration["API_BaseUrl"] ?? "https://localhost:7256")
});

// Register HttpClient for API calls
// This assumes the API is running on https://localhost:7216
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7216/")
});

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
