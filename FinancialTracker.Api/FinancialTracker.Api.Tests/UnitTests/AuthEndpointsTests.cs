using FinancialTracker.Api.Dtos;
using FinancialTracker.Api.Endpoints;
using FinancialTracker.Api.Model;
using FinancialTracker.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace FinancialTracker.Api.Tests;

public class AuthEndpointsTests
{
    [Fact]
    public async void LoginSuccess()
    {
        IAuthenicationService authenicationService = Substitute.For<IAuthenicationService>();
        LoginRequest loginRequest = new() { Email = "test@test.com", Password = "passwordTest" };

        authenicationService.LoginAsync(Arg.Any<LoginRequest>())
            .Returns(Task.FromResult(new LoginResponse() { Success = true}));

        var response = await AuthEndoints.LoginUser(loginRequest, authenicationService);

        Assert.IsType<Ok<LoginResponse>>(response);
        Assert.NotNull(((Ok<LoginResponse>)response).Value);
    }

    [Fact]
    public async void LoginFailure()
    {
        IAuthenicationService authenicationService = Substitute.For<IAuthenicationService>();
        LoginRequest loginRequest = new() { Email = "test@test.com", Password = "passwordTest" };

        authenicationService.LoginAsync(Arg.Any<LoginRequest>())
            .Returns(Task.FromResult(new LoginResponse() { Success = false}));

        var response = await AuthEndoints.LoginUser(loginRequest, authenicationService);

        Assert.IsType<BadRequest<LoginResponse>>(response);
        Assert.NotNull(((BadRequest<LoginResponse>)response).Value);
    }

    [Fact]
    public async void CreateRoleSuccess()
    {
        RoleManager<Role> roleManager = MockHelpers.MockRoleManager<Role>();

        roleManager.CreateAsync(Arg.Any<Role>())
            .Returns(Task.FromResult(IdentityResult.Success));

        var response = await AuthEndoints.CreateRole(roleManager, "test");

        await roleManager.Received(1).CreateAsync(Arg.Is<Role>(r => r.Name == "test"));
        Assert.NotNull(response);
    }

    [Fact]
    public async void RegisterUserSuccess()
    {
        IAuthenicationService authService = Substitute.For<IAuthenicationService>();
        RegisterRequest request = new() { Email = "test@email.com", };
        GenericResponse genericResponse = new() { Success = true, Message = "Success" };

        authService.RegisterAsync(Arg.Any<RegisterRequest>())
            .Returns(Task.FromResult(genericResponse));


        var response = await AuthEndoints.RegisterUser(request, authService);

        await authService.Received(1)
            .RegisterAsync(Arg.Is<RegisterRequest>(r => r.Email == request.Email));
        
        Assert.IsType<Ok<GenericResponse>>(response);
        Assert.NotNull(((Ok<GenericResponse>)response).Value);
        Assert.True(((Ok<GenericResponse>)response).Value?.Success);
    }

    [Fact]
    public async void RegisterUserFailure()
    {
        IAuthenicationService authService = Substitute.For<IAuthenicationService>();
        RegisterRequest request = new() { Email = "test@email.com", };
        GenericResponse genericResponse = new() { Success = false, Message = "Failure" };

        authService.RegisterAsync(Arg.Any<RegisterRequest>())
            .Returns(Task.FromResult(genericResponse));


        var response = await AuthEndoints.RegisterUser(request, authService);

        await authService.Received(1)
            .RegisterAsync(Arg.Is<RegisterRequest>(r => r.Email == request.Email));
        
        Assert.IsType<BadRequest<GenericResponse>>(response);
        Assert.NotNull(((BadRequest<GenericResponse>)response).Value);
        Assert.False(((BadRequest<GenericResponse>)response).Value?.Success);
    }
}
