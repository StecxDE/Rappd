namespace Rappd.CQRS.Tests;

public class ParameterizedCommandWithResultTest
{
    [Fact]
    public async Task SendAsync_Successful_ReturnsParameter()
    {
        // Arrange
        int parameter = 0;
        ParameterizedTestCommandWithResultHandler.Result = i => i;

        // Act
        var response = await ParameterizedTestCommandWithResult.SendAsync(parameter, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.IsSuccess, "The query was not successful.");
        Assert.Equal(parameter, response.Result);
    }
    [Fact]
    public async Task SendAsync_Failing_ReturnsUnknownErrorResult()
    {
        // Arrange
        int parameter = 0;
        ParameterizedTestCommandWithResultHandler.Result = i => i == 0 ? CommonResults.Results.Failed : i;

        // Act
        var response = await ParameterizedTestCommandWithResult.SendAsync(parameter, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(response.IsSuccess, "The query was successful.");
        Assert.IsType<UnknownErrorResult>(response.Error);
    }
    [Fact]
    public async Task SendAsync_Exception_ReturnsExceptionResult()
    {
        // Arrange
        int parameter = 0;
        ParameterizedTestCommandWithResultHandler.Result = i => i == 0 ? throw new Exception() : i;

        // Act
        var response = await ParameterizedTestCommandWithResult.SendAsync(parameter, TestContext.Current.CancellationToken);

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
        var response = await ParameterizedTestCommandWithResult.SendAsync(parameter, cts.Token);

        // Assert
        Assert.False(response.IsSuccess, "The query was successful.");
        Assert.IsType<CancelledResult>(response.Error);
    }
}