namespace Rappd.CQRS.Tests;

public class CommandTest
{
    [Fact]
    public async Task SendAsync_Successful_ReturnsOkResult()
    {
        // Arrange
        TestCommandHandler.Result = () => CQRS.Results.Ok;

        // Act
        var response = await TestCommand.SendAsync();

        // Assert
        Assert.True(response.IsSuccess, "The command was not successful.");
        Assert.IsType<OkResult>(response.Result);
    }
    [Fact]
    public async Task SendAsync_Failing_ReturnsUnknownErrorResult()
    {
        // Arrange
        TestCommandHandler.Result = () => CQRS.Results.Error;

        // Act
        var response = await TestCommand.SendAsync();

        // Assert
        Assert.False(response.IsSuccess, "The command was successful.");
        Assert.IsType<UnknownErrorResult>(response.Error);
    }
    [Fact]
    public async Task SendAsync_Exception_ReturnsExceptionResult()
    {
        // Arrange
        TestCommandHandler.Result = () => throw new Exception();

        // Act
        var response = await TestCommand.SendAsync();

        // Assert
        Assert.False(response.IsSuccess, "The command was successful.");
        Assert.IsType<ExceptionResult>(response.Error);
    }
    [Fact]
    public async Task SendAsync_Cancelled_ReturnsCancelledResult()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var response = await TestCommand.SendAsync(cts.Token);

        // Assert
        Assert.False(response.IsSuccess, "The command was successful.");
        Assert.IsType<CancelledResult>(response.Error);
    }
}
