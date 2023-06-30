using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using CsvHelper;
using CsvHelper.Configuration;
using FinancialTracker.Api.Mappers.CsvMappers;
using FinancialTracker.Api.Model;
using Microsoft.AspNetCore.Identity;

namespace FinancialTracker.Api.Endpoints;

public static class FinancialEndpoints
{
    public static void MapAppEndpoints(this WebApplication app)
    {
        app.MapPost("/copilotupload", async (IFormFile file, 
            CopilotOnboardService onboardService , 
            HttpContext httpContext,
            AuthenicationService authenicationService) =>
        {
            User? user = await authenicationService.GetCurrentUserAsync(httpContext);
            if (user is null)
            {
                return Results.Unauthorized();
            }

            try
            {
                await onboardService.OnboardFromCopilotCsv(user, file);
            }
            catch (System.Exception)
            {
                throw new Exception("Error uploading your csv");
            }
            
            return Results.Content("Transactions and accounts added");
        })
        .WithName("PostCopilotTransactions")
        .RequireAuthorization("user_basic");
    }
}