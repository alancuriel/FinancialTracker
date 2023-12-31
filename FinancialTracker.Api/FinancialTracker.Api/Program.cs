﻿using System.Text.Json.Serialization;
using FinancialTracker.Api;
using FinancialTracker.Api.Configuration;
using FinancialTracker.Api.Endpoints;
using FinancialTracker.Api.Helpers;
using FinancialTracker.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.VerifySecrets();

builder.Services.AddHealthChecks();

builder.Services
        .AddAuthorizationBuilder()
        .AddPolicy(AuthorizationRoles.USER_POLICY, policy =>
                policy.RequireRole(AuthorizationRoles.USER_ROLE));

MongoDbConfiguration.ConfigureMongoDbServices(builder.Configuration, builder.Services);

builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("corsPolicy", builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
        });
    }
    else
    {
        options.AddPolicy("corsPolicy", builder =>
        {
            builder.WithOrigins("https://financial-tracker-taupe.vercel.app",
                    "https://financial-tracker-git-main-alancuriel.vercel.app")
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
        });
    }
});


builder.Services
    .AddScoped<IAuthenicationService, AuthenicationService>()
    .AddScoped<ICopilotOnboardService, CopilotOnboardService>()
    .AddSingleton<IFinancialDataAccess, FinancialDataAccess>()
    .AddSingleton<IFinancialDataService, FinancialDataService>()
    .ConfigureHttpJsonOptions(o =>
    {
        o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        o.SerializerOptions.DefaultIgnoreCondition = 
            JsonIgnoreCondition.WhenWritingNull;
    })
    .Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o =>
    {
        // Configured to support swagger enums as strings 
        // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2293
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        o.JsonSerializerOptions.DefaultIgnoreCondition = 
            JsonIgnoreCondition.WhenWritingNull;
    });

builder.Logging.AddConsole();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition("Bearer", ApiSpecHelper.SecurityScheme);
    o.AddSecurityRequirement(ApiSpecHelper.SecurityReq);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseCors("corsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz");

app.MapAuthApis();
app.MapAppEndpoints();



app.Run();