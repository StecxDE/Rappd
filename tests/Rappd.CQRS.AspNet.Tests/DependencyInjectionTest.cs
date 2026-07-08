using Rappd.CQRS.AspNet.Tests.Classes;

namespace Rappd.CQRS.AspNet.Tests;

public class DependencyInjectionTest
{
    private readonly TestServerFixture _testServerFixture;

    public DependencyInjectionTest(TestServerFixture testServerFixture)
    {
        _testServerFixture = testServerFixture;
    }

    [Fact]
    public async Task DependencyInjection_Successful_ReturnsTestServiceResult()
    {
        // Arrange
        var testService = _testServerFixture.TestServer.Services.GetRequiredService<ITestService>();

        // Act
        var response = await TestQuery.SendAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.IsSuccess, "The query was not successful.");
        Assert.Equal(testService.Result, response.Result);
    }
}