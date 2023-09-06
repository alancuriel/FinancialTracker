using FinancialTracker.Api.Dtos;
using FinancialTracker.Api.Model;
using FinancialTracker.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace FinancialTracker.Api.Tests.UnitTests;

public class AuthenicationServiceTests
{

    [Fact]
    public async Task RegisterEmailInUseFailsTest()
    {
        UserManager<User> userManagerMock = MockHelpers.MockUserManager<User>();
        IConfiguration configurationMock = Substitute.For<IConfiguration>();
        AuthenicationService authService = new(userManagerMock, configurationMock);

        userManagerMock
            .FindByEmailAsync(Arg.Any<String>())
            .Returns(Task.FromResult<User?>(new User()));

        GenericResponse response = await authService.RegisterAsync(GetRegisterRequest());

        Assert.False(response.Success);
        Assert.Contains("Email is already in use", response.Message);
    }

    [Fact]
    public async Task RegisterUserCreationFailsTest()
    {
        UserManager<User> userManagerMock = MockHelpers.MockUserManager<User>();
        IConfiguration configurationMock = Substitute.For<IConfiguration>();
        AuthenicationService authService = new(userManagerMock, configurationMock);

        
        userManagerMock
            .FindByEmailAsync(Arg.Any<string>())
            .Returns(Task.FromResult<User?>(null));

        userManagerMock
            .CreateAsync(Arg.Any<User>(), Arg.Any<string>())
            .Returns(Task.FromResult<IdentityResult>(IdentityResult.Failed()));


        GenericResponse response = await authService.RegisterAsync(GetRegisterRequest());

        Assert.False(response.Success);
        Assert.Contains("Create User failed", response.Message);
    }

    [Fact]
    public async Task RegisterAddRoleToUserFailsTest()
    {
        UserManager<User> userManagerMock = MockHelpers.MockUserManager<User>();
        IConfiguration configurationMock = Substitute.For<IConfiguration>();
        AuthenicationService authService = new(userManagerMock, configurationMock);

        
        userManagerMock
            .FindByEmailAsync(Arg.Any<string>())
            .Returns(Task.FromResult<User?>(null));

        userManagerMock
            .CreateAsync(Arg.Any<User>(), Arg.Any<string>())
            .Returns(Task.FromResult<IdentityResult>(IdentityResult.Success));

        userManagerMock
            .AddToRoleAsync(Arg.Any<User>(), Arg.Any<string>())
            .Returns(Task.FromResult<IdentityResult>(IdentityResult.Failed()));


        GenericResponse response = await authService.RegisterAsync(GetRegisterRequest());

        Assert.False(response.Success);
        Assert.Contains("Created User, but could not add user to role", response.Message);
    }

    [Fact]
    public async Task RegisterSucceedsTest()
    {
        UserManager<User> userManagerMock = MockHelpers.MockUserManager<User>();
        IConfiguration configurationMock = Substitute.For<IConfiguration>();
        AuthenicationService authService = new(userManagerMock, configurationMock);

        
        userManagerMock
            .FindByEmailAsync(Arg.Any<string>())
            .Returns(Task.FromResult<User?>(null));

        userManagerMock
            .CreateAsync(Arg.Any<User>(), Arg.Any<string>())
            .Returns(Task.FromResult<IdentityResult>(IdentityResult.Success));

        userManagerMock
            .AddToRoleAsync(Arg.Any<User>(), Arg.Any<string>())
            .Returns(Task.FromResult<IdentityResult>(IdentityResult.Success));


        GenericResponse response = await authService.RegisterAsync(GetRegisterRequest());

        Assert.True(response.Success);
        Assert.Contains("User registered successfuly!", response.Message);
    }

    [Fact]
    public async Task RegisterExceptionThrownFails()
    {
        UserManager<User> userManagerMock = MockHelpers.MockUserManager<User>();
        IConfiguration configurationMock = Substitute.For<IConfiguration>();
        AuthenicationService authService = new(userManagerMock, configurationMock);
        Exception exception = new("error");

        userManagerMock
            .FindByEmailAsync(Arg.Any<string>())
            .Returns(Task.FromResult<User?>(null));

        userManagerMock
            .CreateAsync(Arg.Any<User>(), Arg.Any<string>())
            .Returns(Task.FromResult<IdentityResult>(IdentityResult.Success));

        userManagerMock
            .AddToRoleAsync(Arg.Any<User>(), Arg.Any<string>())
            .TaskThrows(exception);


        GenericResponse response = await authService.RegisterAsync(GetRegisterRequest());

        Assert.False(response.Success);
        Assert.Contains(exception.Message, response.Message);
    }

    private static RegisterRequest GetRegisterRequest()
    {
        return new RegisterRequest
        {
            FirstName = "Joe",
            LastName = "Bob",
            Email = "joebob@gmail.com",
            Password = "password123!",
            ConfirmPassword = "password123!"
        };
    }
}