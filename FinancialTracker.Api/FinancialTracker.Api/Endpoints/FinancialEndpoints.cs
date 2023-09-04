using FinancialTracker.Api.Dtos;
using FinancialTracker.Api.Model;
using FinancialTracker.Api.Services;

namespace FinancialTracker.Api.Endpoints;

public static class FinancialEndpoints
{
    private const int MAX_RECENT_TRANSACTION_DAYS = 31;

    public const string COPILOT_UPLOAD_URL = "/copilotupload";
    public const string RECENT_TRANSACTIONS_URL = "/v1/financial/recent-transactions/";

    public static void MapAppEndpoints(this WebApplication app)
    {
        app.MapPost(COPILOT_UPLOAD_URL, OnboardCsvFile)
            .WithName("PostCopilotTransactions")
            .RequireAuthorization("user_basic");

        app.MapGet(RECENT_TRANSACTIONS_URL + "{days}", GetRecentTransactions)
            .WithName("RecentTransactions")
            .RequireAuthorization("user_basic");
    }

    public static async Task<IResult> GetRecentTransactions(int days,
        IAuthenicationService authenicationService, HttpContext httpContext,
        FinancialDataService service, ILogger<FinancialEndpoint> logger)
    {
        User? user;

        try
        {
            user = await authenicationService.GetCurrentUserAsync(httpContext);
            if (user is null)
            {
                return Results.Unauthorized();
            }
        }
        catch (Exception ex)
        {
            logger.LogError("An error occured while retrieving User", ex);
            throw;
        }


        if (days > MAX_RECENT_TRANSACTION_DAYS)
        {
            GenericResponse response = new()
            { Success = false, Message = $"Maximumn days allowed is {MAX_RECENT_TRANSACTION_DAYS}" };
            return Results.BadRequest(response);
        }

        List<Transaction> transactions;

        try
        {
            transactions = await service.GetRecentTransactionsAsync(user, days);
        }
        catch (Exception)
        {
            GenericResponse response = new()
            { Success = false, Message = "Error Retrieving Recent transactions." };
            return Results.BadRequest(response);
        }

        return Results.Ok(transactions);
    }

    public static async Task<IResult> OnboardCsvFile(IFormFile file,
        ICopilotOnboardService onboardService, HttpContext httpContext,
        IAuthenicationService authenicationService, ILogger<FinancialEndpoint> logger)
    {

        try
        {
            User? user = await authenicationService.GetCurrentUserAsync(httpContext);
            if (user is null)
            {
                return Results.Unauthorized();
            }
            await onboardService.OnboardFromCopilotCsv(user, file);
        }
        catch (Exception ex)
        {
            const string errorMsg = "Error parsing csv";
            logger.LogError(errorMsg, ex);
            return Results.BadRequest(new GenericResponse { Success = false, Message = errorMsg });
        }

        return Results.Created("/copilotupload", "Transactions and accounts added");
    }
}

public abstract class FinancialEndpoint
{

}