using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FinancialTracker.Api.Mappers.CsvMappers;
using FinancialTracker.Api.Model;
using Microsoft.AspNetCore.Identity;

namespace FinancialTracker.Api;

public class CopilotOnboardService
{
    private readonly CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture)
    {
        NewLine = Environment.NewLine,
        PrepareHeaderForMatch = (args) => args.Header.ToLower(),
    };
    private readonly FinancialDataAccess dataAccess;
    private readonly UserManager<User> userManager;

    public CopilotOnboardService(FinancialDataAccess dataAccess, UserManager<User> userManager)
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
        Dictionary<int, Account> accountMasks = new();

        await foreach (var record in csv.GetRecordsAsync<CopilotTransaction>())
        {
            Transaction newTransaction = new()
            {
                Name = record.Name,
                Amount = record.Amount,
                Date = DateTime.Parse(record.Date),
                Excluded = record.Excluded,
                Note = record.Note,
                Status = record.Status
            };

            if (accountMasks.TryGetValue(record.AccountMask, out Account? account))
            {
                account.Transactions.Add(newTransaction.Id);
            }
            else
            {
                Account newAcc = new()
                {
                    Name = record.Account,
                    Balance = 0,
                };

                newAcc.Transactions.Add(newTransaction.Id);
                user.Accounts.Add(newTransaction.Id);

                accountMasks.Add(record.AccountMask, newAcc);
            }

            transactions.Add(newTransaction);
        }

        await Task.WhenAll(userManager.UpdateAsync(user),
            dataAccess.CreateTransactions(transactions),
            dataAccess.CreateAccountsAsync(accountMasks.ToList()));
    }
}
