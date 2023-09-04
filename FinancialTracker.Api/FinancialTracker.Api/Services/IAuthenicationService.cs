using FinancialTracker.Api.Dtos;
using FinancialTracker.Api.Model;

namespace FinancialTracker.Api.Services;

public interface IAuthenicationService
{
    public Task<GenericResponse> RegisterAsync(RegisterRequest registerRequest);
    public Task<LoginResponse> LoginAsync(LoginRequest request);
    public Task<User?> GetCurrentUserAsync(HttpContext httpContext);
}