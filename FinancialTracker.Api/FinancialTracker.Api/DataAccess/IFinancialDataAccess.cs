namespace FinancialTracker.Api;

public interface IFinancialDataAccess
{
    Task<Account> GetAccountAsync(Guid accountId);
    Task<List<Transaction>> GetTransactionsAsync(Account account);
    Task<List<string>> CreateTransactions(IEnumerable<Transaction> transactions);
    Task CreateAccountsAsync(IEnumerable<Account> accounts);
    Task CreateCategoriesAsync(IEnumerable<Category> categories);
    Task<Guid> CreateAccount(Account account);
    Task<List<Transaction>> GetTransactionsAfterDateAsync(Guid userId, DateTime date);
}