using System.Text;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using FinancialTracker.Api.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace FinancialTracker.Api.Configuration;

public static class MongoDbConfiguration
{
    public static void ConfigureMongoDbServices(
        IConfiguration configuration,
        IServiceCollection services)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

        MongoDbIdentityConfiguration mongoDbIdentityConfiguration = new()
        {
            MongoDbSettings = new MongoDbSettings
            {
                ConnectionString = configuration["MONGO_CONN_NAME"],
                DatabaseName = FinancialDataAccess.DatabaseName
            },
            IdentityOptionsAction = (IdentityOptions options) =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;

                //lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.User.RequireUniqueEmail = true;
            }
        };

        services.ConfigureMongoDbIdentity<User, Role, Guid>(mongoDbIdentityConfiguration)
            .AddUserManager<UserManager<User>>()
            .AddSignInManager<SignInManager<User>>()
            .AddRoleManager<RoleManager<Role>>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = "https://localhost:5001",
                ValidAudience = "https://localhost:5001",
                IssuerSigningKey = 
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT_SECRET_KEY"]!)),
                ClockSkew = TimeSpan.Zero,
            };
        });

    }
}