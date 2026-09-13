using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace AniListingFront.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private const string TokenKey = "anilisting_auth_token";
    private readonly AuthenticationState _anonymous;

    public CustomAuthStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);

            if (string.IsNullOrWhiteSpace(token))
            {
                return _anonymous;
            }

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            return _anonymous;
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);

        using var doc = JsonDocument.Parse(jsonBytes);
        foreach (var property in doc.RootElement.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.Array)
            {
                if (property.Name == "unique_name" || property.Name == ClaimTypes.Name)
                {
                    var first = property.Value.EnumerateArray().FirstOrDefault();
                    var nameVal = first.ValueKind == JsonValueKind.String ? first.GetString() : first.ToString();
                    if (!string.IsNullOrEmpty(nameVal))
                    {
                        claims.Add(new Claim(ClaimTypes.Name, nameVal));
                    }
                }
                else if (property.Name == "sub" || property.Name == ClaimTypes.NameIdentifier)
                {
                    var first = property.Value.EnumerateArray().FirstOrDefault();
                    var subVal = first.ValueKind == JsonValueKind.String ? first.GetString() : first.ToString();
                    if (!string.IsNullOrEmpty(subVal))
                    {
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, subVal));
                    }
                }
                else
                {
                    foreach (var element in property.Value.EnumerateArray())
                    {
                        var val = element.ValueKind == JsonValueKind.String ? element.GetString() : element.ToString();
                        if (!string.IsNullOrEmpty(val))
                        {
                            claims.Add(new Claim(property.Name, val));
                        }
                    }
                }
            }
            else
            {
                var val = property.Value.ValueKind == JsonValueKind.String ? property.Value.GetString() : property.Value.ToString();
                if (val != null)
                {
                    if (property.Name == "unique_name" || property.Name == ClaimTypes.Name)
                    {
                        claims.Add(new Claim(ClaimTypes.Name, val));
                    }
                    else if (property.Name == "sub" || property.Name == ClaimTypes.NameIdentifier)
                    {
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, val));
                    }
                    else
                    {
                        claims.Add(new Claim(property.Name, val));
                    }
                }
            }
        }

        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64.Replace('-', '+').Replace('_', '/'));
    }
}
