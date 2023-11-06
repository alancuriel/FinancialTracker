using System.Linq.Expressions;
using FinancialTracker.Api.Model;
using MongoDB.Driver;

namespace FinancialTracker.Api;

public class FinancialDataAccess : IFinancialDataAccess
{
    public const string DatabaseName = "financialTrackerDB";
    private const string AccountsCollection = "accounts";
    private const string TransactionsCollection = "transactions";
    private const string CategoriesCollection = "categories";

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

    public async Task<IEnumerable<Account>> GetAccountsAsync(IEnumerable<Guid> guids)
    {
        IMongoCollection<Account> accountCollection = ConnectToMongo<Account>(AccountsCollection);

        FilterDefinition<Account> filter = Builders<Account>
            .Filter.In(a => a.Id, guids);

        var results = await accountCollection.FindAsync(filter);

        return results.ToEnumerable();
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

    public async Task<IEnumerable<Category>> GetCategoriesAsync(IEnumerable<Guid> guids)
    {
        IMongoCollection<Category> categoryCollection = 
            ConnectToMongo<Category>(CategoriesCollection);

        FilterDefinition<Category> filter = Builders<Category>
            .Filter.In(c => c.Id, guids);

        var results = await categoryCollection.FindAsync(filter);

        return results.ToEnumerable();
    }

    public async Task<List<string>> CreateTransactions(IEnumerable<Transaction> transactions)
    {
        IMongoCollection<Transaction> transactionCollection =
            ConnectToMongo<Transaction>(TransactionsCollection);

        var trasnactionsToWrite = transactions.Select(t => new InsertOneModel<Transaction>(t));
        var results = await transactionCollection.BulkWriteAsync(trasnactionsToWrite);

        return transactions.Select(t => t.Id.ToString()).ToList();
    }

    public async Task CreateAccountsAsync(IEnumerable<Account> accounts)
    {
        IMongoCollection<Account> accountsCollection =
            ConnectToMongo<Account>(AccountsCollection);

        var accountsToWrite = accounts.Select(acc => new InsertOneModel<Account>(acc));
        var results = await accountsCollection.BulkWriteAsync(accountsToWrite);
    }

    public async Task CreateCategoriesAsync(IEnumerable<Category> categories)
    {
        IMongoCollection<Category> categoriesCollection =
            ConnectToMongo<Category>(CategoriesCollection);

        var accountsToWrite = categories.Select(cat => new InsertOneModel<Category>(cat));
        var results = await categoriesCollection.BulkWriteAsync(accountsToWrite);
    }

    public async Task<Guid> CreateAccount(Account account)
    {
        var accountCollection = ConnectToMongo<Account>(AccountsCollection);

        await accountCollection.InsertOneAsync(account);
        return account.Id;
    }

    public async Task<List<Transaction>> GetTransactionsAfterDateAsync(Guid userId, DateTime date)
    {
        IMongoCollection<Transaction> transactionCollection =
            ConnectToMongo<Transaction>(TransactionsCollection);

        FilterDefinitionBuilder<Transaction> builder =
            Builders<Transaction>.Filter;

        FilterDefinition<Transaction> filter =
            builder.Eq(t => t.UserId, userId) & builder.Gt(t => t.Date, date);

        var results = await transactionCollection.FindAsync(filter);

        return results.ToList();
    }

    private IMongoCollection<T> ConnectToMongo<T>(in string collection)
    {
        MongoClient client = new(configuration["MONGO_CONN_NAME"]);
        IMongoDatabase db = client.GetDatabase(DatabaseName);
        return db.GetCollection<T>(collection);
    }

    public async Task UpdateAccountPrimaryFieldsAsync(Guid id, 
        string? name = null, AccountType? accountType = null)
    {
        if (name is null && accountType is null)
        {
            await Task.CompletedTask;
            return;
        }

        IMongoCollection<Account> accountCollection =
            ConnectToMongo<Account>(AccountsCollection);

        var filter = Builders<Account>.Filter.Eq(a => a.Id, id);

        List<UpdateDefinition<Account>> updates = new();

        if (name is not null)
        {
            updates.Add(Builders<Account>.Update.Set(a => a.Name, name));
        }

        if (accountType is not null)
        {
            updates.Add(Builders<Account>.Update.Set(a => a.Type, accountType));
        }



        var result = await accountCollection
            .UpdateOneAsync(filter, Builders<Account>.Update.Combine(updates
            ));
    }
}
