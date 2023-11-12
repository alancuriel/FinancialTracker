using FinancialTracker.Api.Dtos;
using FinancialTracker.Api.Helpers;
using FinancialTracker.Api.Model;
using FinancialTracker.Api.Services;


namespace FinancialTracker.Api.Endpoints;

public static class FinancialEndpoints
{
    private const int MAX_RECENT_TRANSACTION_DAYS = 31;

    public const string COPILOT_UPLOAD_URL = "/copilotupload";
    public const string RECENT_TRANSACTIONS_URL = "/v1/financial/recent-transactions";
    public const string ACCOUNTS_URL = "/v1/financial/account";
    public const string CATEGORIES_BATCH_URL = "/v1/financial/categories";
    public const string DASHBOARD_URL = "/v1/financial/dashboard";


    public static void MapAppEndpoints(this WebApplication app)
    {
        app.MapPost(COPILOT_UPLOAD_URL, OnboardCsvFile)
            .WithOpenApi(ApiSpecHelper.CopilotUpload)
            .Produces<IList<Account>>()
            .RequireAuthorization("user_basic");

        app.MapGet(CATEGORIES_BATCH_URL, GetAllCategories)
            .Produces<IList<Category>>()
            .RequireAuthorization("user_basic");

        app.MapGet(DASHBOARD_URL, GetDashboardData)
            .Produces<DashboardResponse>()
            .RequireAuthorization("user_basic");

        app.MapPatch(ACCOUNTS_URL, UpdateAccounts)
            .RequireAuthorization("user_basic");

        app.MapGet(RECENT_TRANSACTIONS_URL + "/{days}", GetRecentTransactions)
            .WithOpenApi(ApiSpecHelper.RecentTransactions)
            .Produces<List<Transaction>>()
            .RequireAuthorization("user_basic");
    }

    public static async Task<IResult> GetDashboardData(IAuthenicationService authenicationService,
        HttpContext httpContext, IFinancialDataService service,
        ILogger<FinancialEndpoint> logger)
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


        DashboardResponse response;

        try
        {
            response = await service.GetDashboardData(user);
        }
        catch (Exception ex)
        {
            logger.LogError("An error occured while retrieving dashboard", ex);
            throw;
        }

        return Results.Ok(response);
    }


    public static async Task<IResult> GetAllCategories(IAuthenicationService authenicationService,
        HttpContext httpContext, IFinancialDataService service,
        ILogger<FinancialEndpoint> logger)
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


        IEnumerable<Category> categories;

        try
        {
            categories = await service.GetCategoriessAsync(user);
        }
        catch (Exception)
        {
            GenericResponse response = new()
            { Success = false, Message = "Error retrieving categories." };
            return Results.BadRequest(response);
        }

        return Results.Ok(categories);
    }

    public static async Task<IResult> GetRecentTransactions(int days,
        IAuthenicationService authenicationService, HttpContext httpContext,
        IFinancialDataService service, ILogger<FinancialEndpoint> logger)
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

    public static async Task<IResult> UpdateAccounts(List<UpdateAccountRequest> accounts,
        IAuthenicationService authenicationService, HttpContext httpContext,
        IFinancialDataService service, ILogger<FinancialEndpoint> logger)
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

        HashSet<Guid> userAccountIds = new(user.Accounts);
        bool userHasAccounts = accounts
            .TrueForAll(a => userAccountIds.Contains(a.Id));

        if (!userHasAccounts)
        {
            GenericResponse response = new()
            { Success = false, Message = "Could not find accounts for user." };
            return Results.NotFound(response);
        }

        await service.UpdateAccountDetails(accounts);

        return Results
            .Ok(new { AccountsUpdated = accounts.Select(a => a.Id).ToArray() });
    }

    public static async Task<IResult> OnboardCsvFile(IFormFile file,
        ICopilotOnboardService onboardService, HttpContext httpContext,
        IAuthenicationService authenicationService, ILogger<FinancialEndpoint> logger,
        IFinancialDataService dataService)
    {
        User? user = null;

        try
        {
            user = await authenicationService.GetCurrentUserAsync(httpContext);
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

        IEnumerable<Account> accounts = await dataService.GetAccountsAsync(user);

        return Results.Created<IEnumerable<Account>>("/copilotupload", accounts);
    }
}

public abstract class FinancialEndpoint
{

}