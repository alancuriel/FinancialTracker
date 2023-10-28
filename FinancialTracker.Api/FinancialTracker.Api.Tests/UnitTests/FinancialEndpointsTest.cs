using Amazon.Runtime.Internal.Util;
using CsvHelper;
using FinancialTracker.Api.Dtos;
using FinancialTracker.Api.Endpoints;
using FinancialTracker.Api.Model;
using FinancialTracker.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

namespace FinancialTracker.Api.Tests;

public class FinancialEndpointsTest
{

    IFormFile formFile = Substitute.For<IFormFile>();
    IAuthenicationService authService = Substitute.For<IAuthenicationService>();
    HttpContext httpContext = Substitute.For<HttpContext>();
    ICopilotOnboardService onboardService = Substitute.For<ICopilotOnboardService>();
    ILogger<FinancialEndpoint> logger = Substitute.For<ILogger<FinancialEndpoint>>();
    IFinancialDataService finService = Substitute.For<IFinancialDataService>();

    private void ResetSubstitutes()
    {
        formFile = Substitute.For<IFormFile>();
        authService = Substitute.For<IAuthenicationService>();
        httpContext = Substitute.For<HttpContext>();
        onboardService = Substitute.For<ICopilotOnboardService>();
        logger = Substitute.For<ILogger<FinancialEndpoint>>();
        finService = Substitute.For<IFinancialDataService>();
    }

    [Fact]
    public async void OnboardFromCopilotCsvUserNotFound()
    {
        ResetSubstitutes();

        User user = new() { FirstName = "" };

        authService.GetCurrentUserAsync(Arg.Any<HttpContext>())
            .Returns(Task.FromResult(null as User));

        var response = await FinancialEndpoints
            .OnboardCsvFile(formFile, onboardService, httpContext, authService, logger, finService);

        await onboardService.Received(0)
           .OnboardFromCopilotCsv(Arg.Any<User>(), Arg.Any<IFormFile>());
        Assert.IsType<UnauthorizedHttpResult>(response);
    }

    [Fact]
    public async void OnboardFromCopilotCsvErrorDuringCSVParsing()
    {
        ResetSubstitutes();

        User user = new() { FirstName = "" };

        authService.GetCurrentUserAsync(Arg.Any<HttpContext>())
            .Returns(Task.FromResult<User?>(new User()));
        onboardService.OnboardFromCopilotCsv(Arg.Any<User>(), Arg.Any<IFormFile>())
            .Returns(t => { throw new Exception(""); });


        var response = await FinancialEndpoints
            .OnboardCsvFile(formFile, onboardService, httpContext, authService, logger, finService);

        await onboardService.Received(1)
           .OnboardFromCopilotCsv(Arg.Any<User>(), Arg.Any<IFormFile>());
        Assert.IsType<BadRequest<GenericResponse>>(response);
        Assert.NotNull(((BadRequest<GenericResponse>)response).Value);
        Assert.False(((BadRequest<GenericResponse>)response).Value?.Success);
    }

    [Fact]
    public async void OnboardFromCopilotCsvSuccess()
    {
        ResetSubstitutes();

        IEnumerable<Account> accounts = new List<Account>();

        authService.GetCurrentUserAsync(Arg.Any<HttpContext>())
            .Returns(Task.FromResult<User?>(new User()));

        finService.GetAccountsAsync(Arg.Any<User>())
            .Returns(Task.FromResult<IEnumerable<Account>>(accounts));


        var response = await FinancialEndpoints
            .OnboardCsvFile(formFile, onboardService, httpContext, authService, logger, finService);

        await onboardService.Received(1)
            .OnboardFromCopilotCsv(Arg.Any<User>(), Arg.Any<IFormFile>());


        Assert.IsType<Created<IEnumerable<Account>>>(response);
        Assert.Equivalent(((Created<IEnumerable<Account>>)response).Value, "Transactions and accounts added");
    }
}
