using FinancialTracker.Api.Model;

namespace FinancialTracker.Api.Services;

public class FinancialDataService
{
    private readonly IFinancialDataAccess dataAccess;

    public FinancialDataService(IFinancialDataAccess dataAccess)
    {
        this.dataAccess = dataAccess;
    }

    public async Task<List<Transaction>> GetRecentTransactionsAsync(User user, int days)
    {
        DateTime date = DateTime.Now.AddDays(-days);
        return await dataAccess.GetTransactionsAfterDateAsync(user.Id, date);
    }
}
