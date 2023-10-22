using System.Linq.Expressions;
using FinancialTracker.Api.Model;

namespace FinancialTracker.Api;

public interface IFinancialDataAccess
{
    //Get
    Task<Account> GetAccountAsync(Guid accountId);
    Task<IEnumerable<Account>> GetAccountsAsync(IEnumerable<Guid> guids);
    Task<List<Transaction>> GetTransactionsAsync(Account account);
    Task<List<Transaction>> GetTransactionsAfterDateAsync(Guid userId, DateTime date);

    //Create
    Task<List<string>> CreateTransactions(IEnumerable<Transaction> transactions);
    Task CreateAccountsAsync(IEnumerable<Account> accounts);
    Task CreateCategoriesAsync(IEnumerable<Category> categories);
    Task<Guid> CreateAccount(Account account);
    
    //Update
    Task UpdateAccountPrimaryFieldsAsync(Guid id, 
        string? name = null, AccountType? accountType = null);
}