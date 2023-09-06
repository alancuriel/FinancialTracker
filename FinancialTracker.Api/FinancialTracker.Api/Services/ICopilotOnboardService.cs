using FinancialTracker.Api.Model;

namespace FinancialTracker.Api.Services;

public interface ICopilotOnboardService
{
    Task OnboardFromCopilotCsv(User user, IFormFile file);
}