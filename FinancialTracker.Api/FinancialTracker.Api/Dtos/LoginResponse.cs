using FinancialTracker.Api.Model;

namespace FinancialTracker.Api.Dtos;

public class LoginResponse
{
    public bool Success { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string? TokenType { get; set; }
    public int? ExpiresIn { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = new();
}