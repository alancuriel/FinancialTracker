using FinancialTracker.Api.Dtos;
using FinancialTracker.Api.Model;
using FinancialTracker.Api.Services;
using Microsoft.AspNetCore.Identity;

namespace FinancialTracker.Api.Endpoints;

public static class AuthEndoints
{
    public static void MapAuthApis(this WebApplication app)
    {
        app.MapPost("api/v1/authenticate/register", RegisterUser)
            .Produces<GenericResponse>();

        app.MapPost("api/v1/authenticate/login", LoginUser)
            .Produces<LoginResponse>();
            
        app.MapPost("api/v1/createRole/{roleName}", CreateRole);
    }

    public static async Task<IResult> CreateRole(RoleManager<Role> roleManager, string roleName)
    {
        var appRole = new Role { Name = roleName };
        var createdRole = await roleManager.CreateAsync(appRole);
        return Results.Ok(new { Message = "Role created successfuly" });
    }

    public static async Task<IResult> RegisterUser(RegisterRequest request, IAuthenicationService authService)
    {
        GenericResponse result = await authService.RegisterAsync(request);

        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    public async static Task<IResult> LoginUser(LoginRequest request, IAuthenicationService authService)
    {
        LoginResponse result = await authService.LoginAsync(request);

        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}