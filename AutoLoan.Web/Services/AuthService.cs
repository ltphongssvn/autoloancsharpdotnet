using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AutoLoan.Web.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly AuthStateService _authState;

    public AuthService(HttpClient http, AuthStateService authState)
    {
        _http = http;
        _authState = authState;
    }

    public async Task<UserDto?> LoginAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("auth/login", new { email, password });
        string? token = null;
        if (token == null && response.Headers.Contains("Authorization"))
            token = response.Headers.GetValues("Authorization").FirstOrDefault()?.Replace("Bearer ", "");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();

        if (token != null) _authState.SetToken(token);
        if (result?.Data != null) _authState.SetUser(result.Data);

        return result?.Data;
    }

    public async Task<UserDto?> SignupAsync(SignupRequest request)
    {
        var response = await _http.PostAsJsonAsync("auth/signup", request);
        string? token = null;
        if (token == null && response.Headers.Contains("Authorization"))
            token = response.Headers.GetValues("Authorization").FirstOrDefault()?.Replace("Bearer ", "");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();

        if (token != null) _authState.SetToken(token);
        if (result?.Data != null) _authState.SetUser(result.Data);

        return result?.Data;
    }

    public async Task LogoutAsync()
    {
        try { await _http.DeleteAsync("auth/logout"); } catch { }
        _authState.Clear();
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        try
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<UserDto>>("auth/me");
            if (result?.Data != null) _authState.SetUser(result.Data);
            return result?.Data;
        }
        catch { _authState.Clear(); return null; }
    }
}

public class AuthStateService
{
    public UserDto? User { get; private set; }
    public string? Token { get; private set; }
    public bool IsAuthenticated => User != null;
    public event Action? OnChange;

    public void SetUser(UserDto user) { User = user; OnChange?.Invoke(); }
    public void SetToken(string token) { Token = token; OnChange?.Invoke(); }
    public void Clear() { User = null; Token = null; OnChange?.Invoke(); }
}

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly AuthStateService _authState;
    public AuthHeaderHandler(AuthStateService authState) => _authState = authState;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (_authState.Token != null)
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authState.Token);
        return base.SendAsync(request, ct);
    }
}

public class UserDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("email")] public string Email { get; set; } = "";
    [JsonPropertyName("first_name")] public string FirstName { get; set; } = "";
    [JsonPropertyName("last_name")] public string LastName { get; set; } = "";
    [JsonPropertyName("phone")] public string? Phone { get; set; }
    [JsonPropertyName("role")] public string Role { get; set; } = "customer";
    [JsonPropertyName("full_name")] public string FullName { get; set; } = "";
}

public class SignupRequest
{
    [JsonPropertyName("first_name")] public string FirstName { get; set; } = "";
    [JsonPropertyName("last_name")] public string LastName { get; set; } = "";
    [JsonPropertyName("phone")] public string? Phone { get; set; }
    [JsonPropertyName("email")] public string Email { get; set; } = "";
    [JsonPropertyName("password")] public string Password { get; set; } = "";
    [JsonPropertyName("password_confirmation")] public string PasswordConfirmation { get; set; } = "";
    [JsonPropertyName("role")] public string Role { get; set; } = "borrower";
}

public class ApiResponse<T>
{
    [JsonPropertyName("status")] public ApiStatus? Status { get; set; }
    [JsonPropertyName("data")] public T? Data { get; set; }
}

public class ApiStatus
{
    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("message")] public string? Message { get; set; }
}
