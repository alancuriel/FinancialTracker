using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FinancialTracker.Api.Mappers.CsvMappers;
using FinancialTracker.Api.Model;
using Microsoft.AspNetCore.Identity;

namespace FinancialTracker.Api.Services;

public class CopilotOnboardService
{
    private readonly CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture)
    {
        NewLine = Environment.NewLine,
        PrepareHeaderForMatch = (args) => args.Header.ToLower(),
    };
    private readonly IFinancialDataAccess dataAccess;
    private readonly UserManager<User> userManager;

    public CopilotOnboardService(IFinancialDataAccess dataAccess, UserManager<User> userManager)
    {
        this.dataAccess = dataAccess;
        this.userManager = userManager;
    }

    public async Task OnboardFromCopilotCsv(User user, IFormFile file)
    {
        using StreamReader reader = new(file.OpenReadStream());
        using CsvReader csv = new(reader, csvConfiguration);

        csv.Context.RegisterClassMap<CopilotTransactionMap>();

        List<Transaction> transactions = new();
        Dictionary<string, Account> accountsByName = new();
        Dictionary<string, Category> categoryByNames = new();

        await foreach (var record in csv.GetRecordsAsync<CopilotTransaction>())
        {
            Transaction newTransaction = MapFromCopilotTransaction(record, user.Id);

            if (!string.IsNullOrEmpty(record.Category))
            {
                AddOrCreateCategory(user, categoryByNames, record, newTransaction);
            }

            AddOrCreateAccount(user, accountsByName, record, newTransaction);

            transactions.Add(newTransaction);
        }

        await Task.WhenAll(userManager.UpdateAsync(user),
            dataAccess.CreateCategoriesAsync(categoryByNames.Values),
            dataAccess.CreateTransactions(transactions),
            dataAccess.CreateAccountsAsync(accountsByName.Values));
    }

    private static Transaction MapFromCopilotTransaction(CopilotTransaction record, Guid userId)
    {

        return new()
        {
            Name = record.Name,
            Amount = record.Amount,
            Date = DateTime.Parse(record.Date),
            Excluded = record.Excluded,
            Note = record.Note,
            Status = record.Status,
            UserId = userId,
            Type = MapFromCopilotTransactionType(record.Type),
            Currency = "USD"
        };
    }

    private static TransactionType MapFromCopilotTransactionType(string type)
    {
        return type switch
        {
            "regular" => TransactionType.Regular,
            "internal transfer" => TransactionType.Transfer,
            "income" => TransactionType.Income,
            _ => throw new Exception("Unknown Copilot Transaction Type."),
        };
    }

    private static void AddOrCreateCategory(User user, Dictionary<string, Category> categoryByNames, CopilotTransaction record, Transaction newTransaction)
    {
        if (categoryByNames.TryGetValue(record.Category, out Category? category))
        {
            newTransaction.CategoryId = category.Id;
        }
        else
        {
            Category newCategory = new()
            {
                Name = record.Category,
                Icon = "😀"
            };
            newTransaction.CategoryId = newCategory.Id;
            categoryByNames.Add(record.Category, newCategory);
            user.Categories.Add(newCategory.Id);
        }
    }

    private static void AddOrCreateAccount(User user, Dictionary<string, Account> accountsByName,
        CopilotTransaction record, Transaction newTransaction)
    {
        if (accountsByName.TryGetValue(record.Account, out Account? account))
        {
            account.Transactions.Add(newTransaction.Id);
            newTransaction.AccountId = account.Id;
        }
        else
        {
            Account newAcc = new()
            {
                Name = record.Account,
                Balance = 0,
            };

            // Add transaction to account and vice versa
            newAcc.Transactions.Add(newTransaction.Id);
            newTransaction.AccountId = newAcc.Id;

            // Add account to user
            user.Accounts.Add(newAcc.Id);

            // Add account to accounts
            accountsByName.Add(record.Account, newAcc);
        }
    }
}
