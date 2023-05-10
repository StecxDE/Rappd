using Microsoft.AspNetCore.TestHost;

namespace Rappd.CQRS.AspNet.Tests;

[Collection(nameof(TestServerCollection))]
public class DependencyInjectionTest
{
    private TestServer _server;

    public DependencyInjectionTest(TestServerFixture fixture)
    {
        _server = fixture.TestServer;
    }

    [Fact]
    public async Task DependencyInjection_Successful_ReturnsTestServiceResult()
    {
        // Arrange
        var testService = _server.Services.GetRequiredService<ITestService>();

        // Act
        var response = await TestQuery.SendAsync();

        // Assert
        Assert.True(response.IsSuccess, "The query was not successful.");
        Assert.Equal(testService.Result, response.Result);
    }
}