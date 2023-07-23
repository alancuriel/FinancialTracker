using FinancialTracker.Api.Dtos;
using FinancialTracker.Api.Model;

namespace FinancialTracker.Api.Endpoints;

public static class FinancialEndpoints
{
    private const int MAX_RECENT_TRANSACTION_DAYS = 31;
    public static void MapAppEndpoints(this WebApplication app)
    {
        app.MapPost("/copilotupload", async (IFormFile file,
            CopilotOnboardService onboardService,
            HttpContext httpContext,
            AuthenicationService authenicationService,
            ILogger<Program> logger) =>
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
                logger.LogError("Error uploading csv", ex);
                throw new Exception("An error occured while uploading your csv file.");
            }
            return Results.Content("Transactions and accounts added");
        })
        .WithName("PostCopilotTransactions")
        .RequireAuthorization("user_basic");

        app.MapGet("/v1/financial/recent-transactions/{days}", 
        async (int days, AuthenicationService authenicationService,
            HttpContext httpContext, FinancialDataService service,
            ILogger<Program> logger) =>
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
        });
    }
}