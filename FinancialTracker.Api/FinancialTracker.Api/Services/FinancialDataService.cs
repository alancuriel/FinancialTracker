using FinancialTracker.Api.Helpers;
using FinancialTracker.Api.Model;
using MongoDB.Driver.Linq;

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

    public async Task<DashboardResponse> GetDashboardData(User user)
    {
        DateTime firstOfCurrentMonth = DateTime.Now.FirstDayOfMonth();
        DateTime firstOfLastMonth = firstOfCurrentMonth.AddMonths(-1);

        var transactionsTask = dataAccess.GetTransactionsAfterDateAsync(user.Id, firstOfLastMonth);
        var categoriesTask = dataAccess.GetCategoriesAsync(user.Categories);

        decimal income = 0.0m;
        decimal incomeLastMonth = 0.0m;
        decimal expenses = 0.0m;
        decimal expensesLastMonth = 0.0m;
        int transactionsThisMonth = 0;
        int transactionsLastMonth = 0;
        Dictionary<Guid, decimal> catExpenses = new();
        decimal[] expesnesThisMontPerDay = new decimal
        [
            DateTime.DaysInMonth(firstOfCurrentMonth.Year, firstOfCurrentMonth.Month)
        ];

        IEnumerable<Transaction> transactions = await transactionsTask;
        List<Transaction> transactionsListThisMonth = new();

        foreach (var transaction in transactions)
        {
            if (transaction.Date >= firstOfCurrentMonth)
            {
                switch (transaction.Type)
                {
                    case TransactionType.Regular:
                        expenses += transaction.Amount;

                        if (transaction.CategoryId is not null)
                        {
                            catExpenses.TryGetValue((Guid)transaction.CategoryId, out decimal spent);
                            catExpenses[(Guid)transaction.CategoryId] = spent + transaction.Amount;
                        }

                        expesnesThisMontPerDay[transaction.Date.Day - 1] += transaction.Amount;
                        break;
                    case TransactionType.Income:
                        income += transaction.Amount;
                        break;
                    default:
                        break;
                }
                transactionsThisMonth += 1;
                transactionsListThisMonth.Add(transaction);
            }
            else
            {
                switch (transaction.Type)
                {
                    case TransactionType.Regular:
                        expensesLastMonth += transaction.Amount;
                        break;
                    case TransactionType.Income:
                        incomeLastMonth += transaction.Amount;
                        break;
                    default:
                        break;
                }
                transactionsLastMonth += 1;
            }
        }


        List<Category> categories = (await categoriesTask).ToList();

        var topCategoryKvp = catExpenses.Count > 0 ?
            catExpenses.MaxBy(kvp => kvp.Value) :
            new KeyValuePair<Guid, decimal>(Guid.Empty, 0.0m);

        string topCategoryName = categories.FirstOrDefault(
                c => c.Id == topCategoryKvp.Key,
                new Category { Name = "None" }
            ).Name;


        return new DashboardResponse()
        {
            Income = income * -1,
            IncomeToLastMonth = incomeLastMonth * -1,
            Expenses = expenses,
            ExpensesToLastMonth = expensesLastMonth,
            TopCategory = topCategoryName,
            AmountSpentOnCategory = topCategoryKvp.Value,
            TransactionsThisMonth = transactionsThisMonth,
            TransactionsLastMonth = transactionsLastMonth,
            SpentThisMonth = expesnesThisMontPerDay
                .Select((v, i) => new SpentInDay { Day = i + 1, Spent = v })
                .ToList(),
            RecentTransactions = transactionsListThisMonth
                .OrderByDescending(d => d.Date)
                .Take(5)
                .Select(t => new DashboardTransaction
                {
                    Id = t.Id,
                    Name = t.Name,
                    Amount = t.Amount,
                    Date = t.Date,
                    Type = t.Type,
                    Category = categories
                        .FirstOrDefault(c => c.Id == t.CategoryId)
                        ?.Name
                })
                .ToList()
        };
    }
}
