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
    [Fact]
    public async void OnboardFromCopilotCsvUserNotFound()
    {
        var formFile = Substitute.For<IFormFile>();
        var authService = Substitute.For<IAuthenicationService>();
        var httpContext = Substitute.For<HttpContext>();
        var onboardService = Substitute.For<ICopilotOnboardService>();
        var logger = Substitute.For<ILogger<FinancialEndpoint>>();
        User user = new() { FirstName = ""};

        authService.GetCurrentUserAsync(Arg.Any<HttpContext>())
            .Returns(Task.FromResult(null as User));

        var response = await FinancialEndpoints
            .OnboardCsvFile(formFile, onboardService, httpContext, authService, logger);
        
         await onboardService.Received(0)
            .OnboardFromCopilotCsv(Arg.Any<User>(), Arg.Any<IFormFile>());
        Assert.IsType<UnauthorizedHttpResult>(response);
    }

    [Fact]
    public async void OnboardFromCopilotCsvErrorDuringCSVParsing()
    {
        var formFile = Substitute.For<IFormFile>();
        var authService = Substitute.For<IAuthenicationService>();
        var httpContext = Substitute.For<HttpContext>();
        var onboardService = Substitute.For<ICopilotOnboardService>();
        var logger = Substitute.For<ILogger<FinancialEndpoint>>();
        User user = new() { FirstName = ""};

        authService.GetCurrentUserAsync(Arg.Any<HttpContext>())
            .Returns(Task.FromResult<User?>(new User()));
        onboardService.OnboardFromCopilotCsv(Arg.Any<User>(), Arg.Any<IFormFile>())
            .Returns(t => {throw new Exception("");});
        

        var response = await FinancialEndpoints
            .OnboardCsvFile(formFile, onboardService, httpContext, authService, logger);
        
         await onboardService.Received(1)
            .OnboardFromCopilotCsv(Arg.Any<User>(), Arg.Any<IFormFile>());
        Assert.IsType<BadRequest<GenericResponse>>(response);
        Assert.NotNull(((BadRequest<GenericResponse>)response).Value);
        Assert.False(((BadRequest<GenericResponse>)response).Value?.Success);
    }

    [Fact]
    public async void OnboardFromCopilotCsvSuccess()
    {
        var formFile = Substitute.For<IFormFile>();
        var authService = Substitute.For<IAuthenicationService>();
        var httpContext = Substitute.For<HttpContext>();
        var onboardService = Substitute.For<ICopilotOnboardService>();
        var logger = Substitute.For<ILogger<FinancialEndpoint>>();

        authService.GetCurrentUserAsync(Arg.Any<HttpContext>())
            .Returns(Task.FromResult<User?>(new User()));

        var response = await FinancialEndpoints
            .OnboardCsvFile(formFile, onboardService, httpContext, authService, logger);

        await onboardService.Received(1)
            .OnboardFromCopilotCsv(Arg.Any<User>(), Arg.Any<IFormFile>());
        
        Assert.IsType<Created<string>>(response);
        Assert.Equivalent( ((Created<string>)response).Value, "Transactions and accounts added");
    }
}
