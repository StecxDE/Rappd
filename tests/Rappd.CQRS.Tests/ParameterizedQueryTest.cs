namespace Rappd.CQRS.Tests;

public class ParameterizedQueryTest
{
    [Fact]
    public async Task SendAsync_Successful_ReturnsParameter()
    {
        // Arrange
        int parameter = 0;
        ParameterizedTestQueryHandler.Result = i => i;

        // Act
        var response = await ParameterizedTestQuery.SendAsync(parameter);

        // Assert
        Assert.True(response.IsSuccess, "The query was not successful.");
        Assert.Equal(parameter, response.Result);
    }
    [Fact]
    public async Task SendAsync_Failing_ReturnsUnknownErrorResult()
    {
        // Arrange
        int parameter = 0;
        ParameterizedTestQueryHandler.Result = i => i == 0 ? CQRS.Results.Error : i;

        // Act
        var response = await ParameterizedTestQuery.SendAsync(parameter);

        // Assert
        Assert.False(response.IsSuccess, "The query was successful.");
        Assert.IsType<UnknownErrorResult>(response.Error);
    }
    [Fact]
    public async Task SendAsync_Exception_ReturnsExceptionResult()
    {
        // Arrange
        int parameter = 0;
        ParameterizedTestQueryHandler.Result = i => i == 0 ? throw new Exception() : i;

        // Act
        var response = await ParameterizedTestQuery.SendAsync(parameter);

        // Assert
        Assert.False(response.IsSuccess, "The query was successful.");
        Assert.IsType<ExceptionResult>(response.Error);
    }
    [Fact]
    public async Task SendAsync_Cancelled_ReturnsCancelledResult()
    {
        // Arrange
        int parameter = 0;
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var response = await ParameterizedTestQuery.SendAsync(parameter, cts.Token);

        // Assert
        Assert.False(response.IsSuccess, "The query was successful.");
        Assert.IsType<CancelledResult>(response.Error);
    }
}