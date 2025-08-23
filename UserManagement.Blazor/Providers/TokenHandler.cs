using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace UserManagement.Blazor.Providers;

public class TokenHandler(IJSRuntime jsRuntime) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Get the token from session storage
        var token = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");

        // Add the token to the request header if it exists
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // Proceed with the request
        return await base.SendAsync(request, cancellationToken);
    }
}

