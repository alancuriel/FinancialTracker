using CsvHelper.TypeConversion;
using FinancialTracker.Api.Model;
using FinancialTracker.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace FinancialTracker.Api.Tests;

public class CopilotOnboardServiceTests
{
    [Fact]
    public async Task OnboardValidCsvSuccess()
    {
        UserManager<User> mockUserManager = MockHelpers.MockUserManager<User>();
        IFinancialDataAccess mockFinDA = Substitute.For<IFinancialDataAccess>();

        CopilotOnboardService service = new(mockFinDA, mockUserManager);
        using var stream = File.OpenRead("assets/validcopilot.csv");


        await service.OnboardFromCopilotCsv(new User(), GetCsvFile(stream));

        await mockFinDA.Received(1).CreateAccountsAsync(Arg.Any<IEnumerable<Account>>());
        await mockFinDA.Received(1).CreateTransactions(Arg.Any<IEnumerable<Transaction>>());
        await mockFinDA.Received(1).CreateCategoriesAsync(Arg.Any<IEnumerable<Category>>());
        await mockUserManager.Received(1).UpdateAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task InvalidTransactionTypCsvFails()
    {
        UserManager<User> mockUserManager = MockHelpers.MockUserManager<User>();
        IFinancialDataAccess mockFinDA = Substitute.For<IFinancialDataAccess>();

        CopilotOnboardService service = new(mockFinDA, mockUserManager);
        using var stream = File.OpenRead("assets/invalidcopilot.csv");


        var ex = await Assert
            .ThrowsAsync<Exception>(() =>
                service.OnboardFromCopilotCsv(new User(), GetCsvFile(stream)));

        Assert.Contains("Unknown Copilot Transaction Type.", ex.Message);
        
        await mockFinDA.Received(0).CreateAccountsAsync(Arg.Any<IEnumerable<Account>>());
        await mockFinDA.Received(0).CreateTransactions(Arg.Any<IEnumerable<Transaction>>());
        await mockFinDA.Received(0).CreateCategoriesAsync(Arg.Any<IEnumerable<Category>>());
        await mockUserManager.Received(0).UpdateAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task InvalidCsvFailsAndThrowsException()
    {
        UserManager<User> mockUserManager = MockHelpers.MockUserManager<User>();
        IFinancialDataAccess mockFinDA = Substitute.For<IFinancialDataAccess>();

        CopilotOnboardService service = new(mockFinDA, mockUserManager);
        using var stream = File.OpenRead("assets/invalidcopilot2.csv");


        var ex = await Assert
            .ThrowsAsync<TypeConverterException>(() =>
                service.OnboardFromCopilotCsv(new User(), GetCsvFile(stream)));

        Assert.Contains("The conversion cannot be performed", ex.Message);
        
        await mockFinDA.Received(0).CreateAccountsAsync(Arg.Any<IEnumerable<Account>>());
        await mockFinDA.Received(0).CreateTransactions(Arg.Any<IEnumerable<Transaction>>());
        await mockFinDA.Received(0).CreateCategoriesAsync(Arg.Any<IEnumerable<Category>>());
        await mockUserManager.Received(0).UpdateAsync(Arg.Any<User>());
    }

    public static IFormFile GetCsvFile(FileStream stream)
    {
        return new FormFile(stream, 0, stream.Length, "", Path.GetFileName(stream.Name))
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/csv"
        };
    }
}
