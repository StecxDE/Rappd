namespace Rappd.CQRS.Tests;

public class ParameterizedCommandTest
{
    [Fact]
    public async Task SendAsync_Successful_ReturnsOkResult()
    {
        // Arrange
        int parameter = 0;
        ParameterizedTestCommandHandler.Result = (i) => i == 0 ? CQRS.Results.Ok : throw new ArgumentException();

        // Act
        var response = await ParameterizedTestCommand.SendAsync(parameter);

        // Assert
        Assert.True(response.IsSuccess, "The command was not successful.");
        Assert.IsType<OkResult>(response.Result);
    }
    [Fact]
    public async Task SendAsync_Failing_ReturnsUnknownErrorResult()
    {
        // Arrange
        int parameter = 0;
        ParameterizedTestCommandHandler.Result = (i) => i == 0 ? CQRS.Results.Error : throw new ArgumentException();

        // Act
        var response = await ParameterizedTestCommand.SendAsync(parameter);

        // Assert
        Assert.False(response.IsSuccess, "The command was successful.");
        Assert.IsType<UnknownErrorResult>(response.Error);
    }
    [Fact]
    public async Task SendAsync_Exception_ReturnsExceptionResult()
    {
        // Arrange
        int parameter = 0;
        ParameterizedTestCommandHandler.Result = (i) => i == 0 ? throw new Exception() : CQRS.Results.Error;

        // Act
        var response = await ParameterizedTestCommand.SendAsync(parameter);

        // Assert
        Assert.False(response.IsSuccess, "The command was successful.");
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
        var response = await ParameterizedTestCommand.SendAsync(parameter, cts.Token);

        // Assert
        Assert.False(response.IsSuccess, "The command was successful.");
        Assert.IsType<CancelledResult>(response.Error);
    }
}
