using Microsoft.OpenApi.Models;

namespace FinancialTracker.Api.Helpers;

public class ApiSpecHelper
{
    public const string COPILOT_UPLOAD_DESC = """ 
    This api is for uploading copilot transactions as part of onboarding a user.
    It will return a list of accounts that either need to be categorized and/or renamed.
    """;

    public const string RECENT_TRANSACTIONS_DESC = """
    Get recent transactions from all accoiunts up to 31 days ago.
    """;

    public static OpenApiOperation CopilotUpload(OpenApiOperation operation)
    {
        return new(operation)
        {
            Description = COPILOT_UPLOAD_DESC,
        };
    }

    public static OpenApiOperation RecentTransactions(OpenApiOperation operation)
    {
        return new(operation)
        {
            Description = RECENT_TRANSACTIONS_DESC,
        };
    }

    public static OpenApiSecurityScheme SecurityScheme { get; } = new()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JSON Web Token based security",
    };

    public static OpenApiSecurityRequirement SecurityReq { get; } = new()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    };


}
