using FinancialTracker.Api.Model;

namespace FinancialTracker.Api.Services;

public class FinancialDataService
{
    private readonly FinancialDataAccess dataAccess;

    public FinancialDataService(FinancialDataAccess dataAccess)
    {
        this.dataAccess = dataAccess;
    }

    public async Task<List<Transaction>> GetRecentTransactionsAsync(User user, int days)
    {
        DateTime date = DateTime.Now.AddDays(-days);
        return await dataAccess.GetTransactionsAfterDateAsync(user.Id, date);
    }
}
