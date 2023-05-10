namespace Rappd.CQRS.Tests;

public class QueryTest
{
    [Fact]
    public async Task SendAsync_Successful_ReturnsZero()
    {
        // Arrange
        TestQueryHandler.Result = () => 0;

        // Act
        var response = await TestQuery.SendAsync();

        // Assert
        Assert.True(response.IsSuccess, "The query was not successful.");
        Assert.Equal(0, response.Result);
    }
    [Fact]
    public async Task SendAsync_Failing_ReturnsUnknownErrorResult()
    {
        // Arrange
        TestQueryHandler.Result = () => CQRS.Results.Error;

        // Act
        var response = await TestQuery.SendAsync();

        // Assert
        Assert.False(response.IsSuccess, "The query was successful.");
        Assert.IsType<UnknownErrorResult>(response.Error);
    }
    [Fact]
    public async Task SendAsync_Exception_ReturnsExceptionResult()
    {
        // Arrange
        TestQueryHandler.Result = () => throw new Exception();

        // Act
        var response = await TestQuery.SendAsync();

        // Assert
        Assert.False(response.IsSuccess, "The query was successful.");
        Assert.IsType<ExceptionResult>(response.Error);
    }
    [Fact]
    public async Task SendAsync_Cancelled_ReturnsCancelledResult()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var response = await TestQuery.SendAsync(cts.Token);

        // Assert
        Assert.False(response.IsSuccess, "The query was successful.");
        Assert.IsType<CancelledResult>(response.Error);
    }
}