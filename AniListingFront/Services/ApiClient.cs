using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AniListingFront.Models;

namespace AniListingFront.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _http;
    private readonly CustomAuthStateProvider _authStateProvider;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ApiClient(HttpClient http, CustomAuthStateProvider authStateProvider)
    {
        _http = http;
        _authStateProvider = authStateProvider;
    }

    private async Task AddAuthHeaderAsync(HttpRequestMessage request)
    {
        var token = await _authStateProvider.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<(bool Success, string? Error, AuthResponseModel? Data)> RegisterAsync(RegisterModel model)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", new
            {
                username = model.Username,
                password = model.Password
            });

            if (response.IsSuccessStatusCode)
            {
                var authData = await response.Content.ReadFromJsonAsync<AuthResponseModel>(_jsonOptions);
                if (authData != null)
                {
                    await _authStateProvider.MarkUserAsAuthenticated(authData.Token);
                    return (true, null, authData);
                }
            }

            var errorObj = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(_jsonOptions);
            var error = errorObj != null && errorObj.TryGetValue("message", out var msg)
                ? msg.ToString()
                : "Registration failed. Please check your credentials.";

            return (false, error, null);
        }
        catch (Exception ex)
        {
            return (false, $"Network error: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string? Error, AuthResponseModel? Data)> LoginAsync(LoginModel model)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new
            {
                username = model.Username,
                password = model.Password
            });

            if (response.IsSuccessStatusCode)
            {
                var authData = await response.Content.ReadFromJsonAsync<AuthResponseModel>(_jsonOptions);
                if (authData != null)
                {
                    await _authStateProvider.MarkUserAsAuthenticated(authData.Token);
                    return (true, null, authData);
                }
            }

            var errorObj = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(_jsonOptions);
            var error = errorObj != null && errorObj.TryGetValue("message", out var msg)
                ? msg.ToString()
                : "Invalid username or password.";

            return (false, error, null);
        }
        catch (Exception ex)
        {
            return (false, $"Network error: {ex.Message}", null);
        }
    }

    public async Task LogoutAsync()
    {
        await _authStateProvider.MarkUserAsLoggedOut();
    }

    public async Task<List<MediaItemModel>> SearchMediaAsync(string mediaType, string query)
    {
        try
        {
            var url = $"api/media/search?type={Uri.EscapeDataString(mediaType)}&q={Uri.EscapeDataString(query)}";
            var result = await _http.GetFromJsonAsync<List<MediaItemModel>>(url, _jsonOptions);
            return result ?? new List<MediaItemModel>();
        }
        catch
        {
            return new List<MediaItemModel>();
        }
    }

    public async Task<MediaDetailsModel?> GetMediaDetailsAsync(string mediaType, int id)
    {
        try
        {
            var url = $"api/media/{Uri.EscapeDataString(mediaType)}/{id}";
            return await _http.GetFromJsonAsync<MediaDetailsModel>(url, _jsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<UserMediaItemModel>> GetUserListAsync(string mediaType, string? status = null, string sortBy = "date", string sortOrder = "desc")
    {
        try
        {
            var url = $"api/userlist?mediaType={Uri.EscapeDataString(mediaType)}&sortBy={Uri.EscapeDataString(sortBy)}&sortOrder={Uri.EscapeDataString(sortOrder)}";
            if (!string.IsNullOrWhiteSpace(status))
            {
                url += $"&status={Uri.EscapeDataString(status)}";
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            await AddAuthHeaderAsync(request);

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var list = await response.Content.ReadFromJsonAsync<List<UserMediaItemModel>>(_jsonOptions);
                return list ?? new List<UserMediaItemModel>();
            }

            return new List<UserMediaItemModel>();
        }
        catch
        {
            return new List<UserMediaItemModel>();
        }
    }

    public async Task<UserMediaItemModel?> CheckUserListItemAsync(string mediaType, int malId)
    {
        try
        {
            var url = $"api/userlist/check/{Uri.EscapeDataString(mediaType)}/{malId}";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            await AddAuthHeaderAsync(request);

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserMediaItemModel>(_jsonOptions);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Success, string? Error, UserMediaItemModel? Item)> SaveUserListItemAsync(SaveUserMediaModel model)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "api/userlist")
            {
                Content = JsonContent.Create(model)
            };
            await AddAuthHeaderAsync(request);

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var item = await response.Content.ReadFromJsonAsync<UserMediaItemModel>(_jsonOptions);
                return (true, null, item);
            }

            return (false, "Could not save item to list.", null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<bool> DeleteUserListItemAsync(string mediaType, int malId)
    {
        try
        {
            var url = $"api/userlist/{Uri.EscapeDataString(mediaType)}/{malId}";
            using var request = new HttpRequestMessage(HttpMethod.Delete, url);
            await AddAuthHeaderAsync(request);

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
