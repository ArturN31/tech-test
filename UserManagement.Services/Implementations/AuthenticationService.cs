using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace UserManagement.Services.Implementations;

/// <summary>
/// A service to manage authentication state, including login and logout logic.
/// It uses a local event to notify components of state changes.
/// </summary>
public class AuthenticationService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly HttpClient _httpClient;
    private string? _authToken;

    /// <summary>
    /// Event that components can subscribe to, to be notified when the authentication state changes.
    /// </summary>
    public event Action? OnAuthStateChanged;

    /// <summary>
    /// Gets a value indicating whether a user is currently logged in.
    /// </summary>
    public bool IsLoggedIn => !string.IsNullOrEmpty(_authToken);

    public AuthenticationService(IJSRuntime jsRuntime, HttpClient httpClient)
    {
        _jsRuntime = jsRuntime;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Checks the user's login status by retrieving the token from session storage.
    /// </summary>
    public async Task CheckLoginStatusAsync()
    {
        _authToken = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");
        OnAuthStateChanged?.Invoke();
    }

    /// <summary>
    /// Handles the user logout process.
    /// </summary>
    public async Task LogoutAsync()
    {
        try
        {
            // Call the backend API to blacklist the token.
            await _httpClient.PostAsJsonAsync("api/AccountManagement/logout", new { });
        }
        catch (Exception ex)
        {
            // Log the exception but continue with client-side logout, as the token is client-managed.
            Console.WriteLine($"Error calling logout API: {ex.Message}");
        }

        // Remove token and expiry from session storage.
        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "authTokenExpiry");

        // Clear the local state.
        _authToken = null;

        // Notify any subscribed components of the state change.
        OnAuthStateChanged?.Invoke();
    }
}
