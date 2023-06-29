namespace FinancialTracker.Api;

public static class StartUpHelper
{
    public static void VerifySecrets(this WebApplicationBuilder builder)
    {
        IConfiguration config = builder.Configuration;

        if (config["MONGO_CONN_NAME"] is null ||
            config["JWT_SECRET_KEY"] is null)
        {
            throw new NullReferenceException("Required Secrets not found");
        }
    }
}
