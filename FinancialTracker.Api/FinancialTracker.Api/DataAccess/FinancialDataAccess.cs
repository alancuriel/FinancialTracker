using MongoDB.Driver;

namespace FinancialTracker.Api;

public class FinancialDataAccess
{
    public const string DatabaseName = "financialTrackerDB";
    private const string AccountsCollection = "accounts";
    private const string TransactionsCollection = "transactions";

    private readonly IConfiguration configuration;

    public FinancialDataAccess(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<Account> GetAccountAsync(Guid accountId)
    {
        var accountCollection = ConnectToMongo<Account>(AccountsCollection);
        var results = await accountCollection.FindAsync(c => c.Id == accountId);
        return results.First();
    }

    public async Task<List<Transaction>> GetTransactionsAsync(Account account)
    {
        IMongoCollection<Transaction> transactionCollection =
            ConnectToMongo<Transaction>(TransactionsCollection);

        FilterDefinition<Transaction> filter = Builders<Transaction>
            .Filter.In(t => t.Id, account.Transactions);

        var results = await transactionCollection.FindAsync(filter);

        return results.ToList();
    }

    public async Task<List<string>> CreateTransactions(List<Transaction> transactions)
    {
        IMongoCollection<Transaction> transactionCollection =
            ConnectToMongo<Transaction>(TransactionsCollection);

        var trasnactionsToWrite = transactions.Select(t => new InsertOneModel<Transaction>(t));
        var results = await transactionCollection.BulkWriteAsync(trasnactionsToWrite);

        return transactions.Select(t => t.Id.ToString()).ToList();
    }

    public async Task CreateAccountsAsync(List<KeyValuePair<int, Account>> accounts)
    {
        IMongoCollection<Account> accountsCollection =
            ConnectToMongo<Account>(AccountsCollection);

        var accountsToWrite = accounts.Select(acc => new InsertOneModel<Account>(acc.Value));
        var results = await accountsCollection.BulkWriteAsync(accountsToWrite);

    }

    public async Task<Guid> CreateAccount(Account account)
    {
        var accountCollection = ConnectToMongo<Account>(AccountsCollection);

        await accountCollection.InsertOneAsync(account);
        return account.Id;
    }

    private IMongoCollection<T> ConnectToMongo<T>(in string collection)
    {
        MongoClient client = new(configuration["MONGO_CONN_NAME"]);
        IMongoDatabase db = client.GetDatabase(DatabaseName);
        return db.GetCollection<T>(collection);
    }

    
}
