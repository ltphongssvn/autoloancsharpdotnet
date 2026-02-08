using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutoLoan.Web.Services;

public class ApplicationDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; } = "";
    [JsonPropertyName("application_number")] public string? ApplicationNumber { get; set; }
    [JsonPropertyName("personal_info")] public JsonElement? PersonalInfo { get; set; }
    [JsonPropertyName("car_details")] public JsonElement? CarDetails { get; set; }
    [JsonPropertyName("loan_details")] public JsonElement? LoanDetails { get; set; }
    [JsonPropertyName("employment_info")] public JsonElement? EmploymentInfo { get; set; }
    [JsonPropertyName("loan_term")] public int? LoanTerm { get; set; }
    [JsonPropertyName("interest_rate")] public double? InterestRate { get; set; }
    [JsonPropertyName("monthly_payment")] public double? MonthlyPayment { get; set; }
    [JsonPropertyName("created_at")] public string CreatedAt { get; set; } = "";
    [JsonPropertyName("updated_at")] public string? UpdatedAt { get; set; }
    [JsonPropertyName("submitted_at")] public string? SubmittedAt { get; set; }
    [JsonPropertyName("decided_at")] public string? DecidedAt { get; set; }
    [JsonPropertyName("signed_at")] public string? SignedAt { get; set; }
    [JsonPropertyName("links")] public Dictionary<string, string>? Links { get; set; }
}

public class ApplicationService
{
    private readonly HttpClient _http;

    public ApplicationService(IHttpClientFactory factory) => _http = factory.CreateClient("API");

    public async Task<List<ApplicationDto>> GetApplicationsAsync()
    {
        var res = await _http.GetAsync("applications");
        if (!res.IsSuccessStatusCode) return new();
        var json = await res.Content.ReadFromJsonAsync<ApiResponse<List<ApplicationDto>>>();
        return json?.Data ?? new();
    }

    public async Task<ApplicationDto?> GetApplicationAsync(int id)
    {
        var res = await _http.GetAsync($"applications/{id}");
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadFromJsonAsync<ApiResponse<ApplicationDto>>();
        return json?.Data;
    }

    public async Task<ApplicationDto?> CreateApplicationAsync(object application)
    {
        var res = await _http.PostAsJsonAsync("applications", new { application });
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadFromJsonAsync<ApiResponse<ApplicationDto>>();
        return json?.Data;
    }

    public async Task<ApplicationDto?> UpdateApplicationAsync(int id, object application)
    {
        var res = await _http.PatchAsJsonAsync($"applications/{id}", new { application });
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadFromJsonAsync<ApiResponse<ApplicationDto>>();
        return json?.Data;
    }

    public async Task<ApplicationDto?> SubmitApplicationAsync(int id)
    {
        var res = await _http.PostAsync($"applications/{id}/submit", null);
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadFromJsonAsync<ApiResponse<ApplicationDto>>();
        return json?.Data;
    }

    public async Task<ApplicationDto?> SignApplicationAsync(int id, string signatureData)
    {
        var res = await _http.PostAsJsonAsync($"applications/{id}/sign", new { signature_data = signatureData });
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadFromJsonAsync<ApiResponse<ApplicationDto>>();
        return json?.Data;
    }

    public async Task<bool> DeleteApplicationAsync(int id)
    {
        var res = await _http.DeleteAsync($"applications/{id}");
        return res.IsSuccessStatusCode;
    }
}
