using FinancialTracker.Api.Model;

namespace FinancialTracker.Api.Services;

public interface IFinancialDataService
{
    Task<List<Transaction>> GetRecentTransactionsAsync(User user, int days);
    Task<IEnumerable<Account>> GetAccountsAsync(User user);
    Task UpdateAccountDetails(IEnumerable<UpdateAccountRequest> accounts);
    Task<IEnumerable<Category>> GetCategoriessAsync(User user);
}