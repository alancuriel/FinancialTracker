using FinancialTracker.Api.Model;

namespace FinancialTracker.Api.Services;

public class FinancialDataService : IFinancialDataService
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

    public async Task<IEnumerable<Account>> GetAccountsAsync(User user)
    {
        var accounts = await dataAccess.GetAccountsAsync(user.Accounts);
        return accounts;
    }

    public async Task<IEnumerable<Category>> GetCategoriessAsync(User user)
    {
        var categories = await dataAccess.GetCategoriesAsync(user.Categories);
        return categories;
    }

    public async Task UpdateAccountDetails(IEnumerable<UpdateAccountRequest> accounts)
    {

        List<Task> tasks = new();
        foreach (var account in accounts)
        {
            tasks.Add(dataAccess.UpdateAccountPrimaryFieldsAsync
            (
                account.Id,
                account.Name,
                account.Type
            ));
        }
        await Task.WhenAll(tasks.ToArray());
    }
}
