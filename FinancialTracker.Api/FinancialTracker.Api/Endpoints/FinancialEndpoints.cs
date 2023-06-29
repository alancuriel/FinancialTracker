using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FinancialTracker.Api.Mappers.CsvMappers;
using FinancialTracker.Api.Model;

namespace FinancialTracker.Api.Endpoints;

public static class FinancialEndpoints 
{
    public static void MapAppEndpoints(this WebApplication app)
    {
        app.MapPost("/copilotupload", async (IFormFile file, FinancialDataAccess dA) =>
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                PrepareHeaderForMatch = (args) => args.Header.ToLower(),
            };

            using StreamReader reader = new(file.OpenReadStream());
            using CsvReader csv = new(reader, csvConfig);

            csv.Context.RegisterClassMap<CopilotTransactionMap>();

            var records = csv.GetRecordsAsync<CopilotTransaction>();
            // await foreach (var record in records)
            // {
            //     if (record.Category == "Car")
            //     {
            //         Console.WriteLine($"Transaction Found ${record.Name} - ${record.Amount}");
            //     }
            // }

            var transactions = records.ToBlockingEnumerable()
                .Where(r => r.Category == "Car")
                .Select(r => new Transaction
                    {
                        Name = r.Name,
                        Amount = r.Amount,
                        Date = DateTime.Parse(r.Date),
                        Excluded = r.Excluded,
                        Note = r.Note,
                        Status = r.Status
                    })
                .ToList();
            return await dA.CreateTransactions(transactions);
        })
        .WithName("PostCopilotTransactions");
        // .RequireAuthorization("user_basic");
    }
}