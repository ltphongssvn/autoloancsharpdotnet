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
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result?.Token != null) _authState.SetToken(result.Token);
        if (result?.User != null) _authState.SetUser(result.User);
        return result?.User;
    }

    public async Task<UserDto?> SignupAsync(SignupRequest request)
    {
        var response = await _http.PostAsJsonAsync("auth/register", new
        {
            email = request.Email,
            password = request.Password,
            firstName = request.FirstName,
            lastName = request.LastName,
            phone = request.Phone ?? ""
        });
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result?.Token != null) _authState.SetToken(result.Token);
        if (result?.User != null) _authState.SetUser(result.User);
        return result?.User;
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
            var result = await _http.GetFromJsonAsync<AuthResponse>("auth/me");
            if (result?.User != null) _authState.SetUser(result.User);
            return result?.User;
        }
        catch { _authState.Clear(); return null; }
    }
}

public class AuthResponse
{
    [JsonPropertyName("token")] public string? Token { get; set; }
    [JsonPropertyName("user")] public UserDto? User { get; set; }
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
    [JsonPropertyName("firstName")] public string FirstName { get; set; } = "";
    [JsonPropertyName("lastName")] public string LastName { get; set; } = "";
    [JsonPropertyName("phone")] public string? Phone { get; set; }
    [JsonPropertyName("role")] public string Role { get; set; } = "customer";
}

public class SignupRequest
{
    [JsonPropertyName("firstName")] public string FirstName { get; set; } = "";
    [JsonPropertyName("lastName")] public string LastName { get; set; } = "";
    [JsonPropertyName("phone")] public string? Phone { get; set; }
    [JsonPropertyName("email")] public string Email { get; set; } = "";
    [JsonPropertyName("password")] public string Password { get; set; } = "";
    [JsonPropertyName("passwordConfirmation")] public string PasswordConfirmation { get; set; } = "";
    [JsonPropertyName("role")] public string Role { get; set; } = "customer";
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
