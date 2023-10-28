namespace FinancialTracker.Api.Model;

public class UserStatus
{
    public bool Onboarded { get; set; } = false;
    public bool WantsCopilotOnboard { get; set; } = false;
}