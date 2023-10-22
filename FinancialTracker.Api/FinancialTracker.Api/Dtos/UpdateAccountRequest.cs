namespace FinancialTracker.Api;

public class UpdateAccountRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public AccountType? Type { get; set; } = null;
}
