using FinancialTracker.Api.Model;
using FinancialTracker.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace FinancialTracker.Api.Tests;

public class CopilotOnboardServiceTests
{
    [Fact]
    public async Task FirstTest()
    {
        Mock<UserManager<User>> mockUserManager = MockHelpers.MockUserManager<User>();
        Mock<IFinancialDataAccess> mockFinDA = new();

        CopilotOnboardService service = new(mockFinDA.Object, mockUserManager.Object);
        using var stream = File.OpenRead("assets/validcopilot.csv");

        mockFinDA.Setup(x => x.CreateCategoriesAsync(It.IsAny<IEnumerable<Category>>()))
            .Returns(Task.CompletedTask);

        mockFinDA.Setup(x => x.CreateTransactions(It.IsAny<IEnumerable<Transaction>>()))
            .ReturnsAsync(new List<string>());

        mockFinDA.Setup(x => x.CreateAccountsAsync(It.IsAny<IEnumerable<Account>>()))
            .Returns(Task.CompletedTask);


        await service.OnboardFromCopilotCsv(new User(), GetCsvFile(stream));
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
