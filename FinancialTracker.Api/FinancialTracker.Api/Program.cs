using FinancialTracker.Api;
using FinancialTracker.Api.Configuration;
using FinancialTracker.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.VerifySecrets();

builder.Services
        .AddAuthorizationBuilder()
        .AddPolicy(AuthorizationRoles.USER_POLICY, policy =>
                policy.RequireRole(AuthorizationRoles.USER_ROLE));
                    
MongoDbConfiguration.ConfigureMongoDbServices(builder.Configuration, builder.Services);

builder.Services
    .AddScoped<AuthenicationService>()
    .AddScoped<CopilotOnboardService>()
    .AddSingleton<FinancialDataAccess>()
    .AddSingleton<FinancialDataService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthApis();
app.MapAppEndpoints();


app.Run();