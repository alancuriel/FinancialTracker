using FinancialTracker.Api.Dtos;
using FinancialTracker.Api.Model;
using Microsoft.AspNetCore.Identity;

namespace FinancialTracker.Api.Endpoints;

public static class AuthEndoints
{
    public static void MapAuthApis(this WebApplication app)
    {
        app.MapPost("api/v1/createRole/{roleName}", async (RoleManager<Role> roleManager, string roleName) =>
        {
            var appRole = new Role { Name = roleName };
            var createdRole = await roleManager.CreateAsync(appRole);
            return Results.Ok(new { Message = "Role created successfuly" });
        });

        app.MapPost("api/v1/authenticate/register", 
            async (RegisterRequest registerRequest, AuthenicationService authService) =>
        {
            RegisterResponse result = await authService.RegisterAsync(registerRequest);

            return result.Success ? Results.Ok(result) : Results.BadRequest(result.Message);
        });


        app.MapPost("api/v1/authenticate/login", 
            async (LoginRequest request, AuthenicationService authService) =>
        {
            LoginResponse result = await authService.LoginAsync(request);

            return result.Success ? Results.Ok(result) : Results.BadRequest(result.Message);
        });


    }
}