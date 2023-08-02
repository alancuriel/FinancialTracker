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
        Mock<UserManager<User>> userManagerMock = MockHelpers.MockUserManager<User>();
        Mock<IConfiguration> configurationMock = new();
        AuthenicationService authService = new(userManagerMock.Object, configurationMock.Object);

        userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());

        GenericResponse response = await authService.RegisterAsync(GetRegisterRequest());

        Assert.False(response.Success);
        Assert.Contains("Email is already in use", response.Message);
    }

    [Fact]
    public async Task RegisterUserCreationFailsTest()
    {
        Mock<UserManager<User>> userManagerMock = MockHelpers.MockUserManager<User>();
        Mock<IConfiguration> configurationMock = new();
        AuthenicationService authService = new(userManagerMock.Object, configurationMock.Object);

        userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(null as User);

        userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());


        GenericResponse response = await authService.RegisterAsync(GetRegisterRequest());

        Assert.False(response.Success);
        Assert.Contains("Create User failed", response.Message);
    }

    [Fact]
    public async Task RegisterAddRoleToUserFailsTest()
    {
        Mock<UserManager<User>> userManagerMock = MockHelpers.MockUserManager<User>();
        Mock<IConfiguration> configurationMock = new();
        AuthenicationService authService = new(userManagerMock.Object, configurationMock.Object);

        userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(null as User);

        userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());


        GenericResponse response = await authService.RegisterAsync(GetRegisterRequest());

        Assert.False(response.Success);
        Assert.Contains("Created User, but could not add user to role", response.Message);
    }

    [Fact]
    public async Task RegisterSucceedsTest()
    {
        Mock<UserManager<User>> userManagerMock = MockHelpers.MockUserManager<User>();
        Mock<IConfiguration> configurationMock = new();
        AuthenicationService authService = new(userManagerMock.Object, configurationMock.Object);

        userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(null as User);

        userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);


        GenericResponse response = await authService.RegisterAsync(GetRegisterRequest());

        Assert.True(response.Success);
        Assert.Contains("User registered successfuly!", response.Message);
    }

    [Fact]
    public async Task RegisterExceptionThrownFails()
    {
        Mock<UserManager<User>> userManagerMock = MockHelpers.MockUserManager<User>();
        Mock<IConfiguration> configurationMock = new();
        AuthenicationService authService = new(userManagerMock.Object, configurationMock.Object);
        Exception exception = new Exception("error");

        userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(null as User);

        userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ThrowsAsync(exception);


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