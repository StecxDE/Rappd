namespace Rappd.CQRS.Tests;

public class HandlerSelectionTest
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task SingleAttribute_Successful_ReturnsValue(int value)
    {
        // Arrange
        TestHandlerSelectionAttribute.Values = new[] { value };

        // Act
        var response = await SingleHandlerSelectionTestQuery.SendAsync();

        // Assert
        Assert.True(response.IsSuccess, "The query was not successful.");
        Assert.Equal(value, response.Result);
    }
    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public async Task SingleAttribute_Failing_ReturnsNoHandlerError(int value)
    {
        // Arrange
        TestHandlerSelectionAttribute.Values = new[] { value };

        // Act
        var response = await SingleHandlerSelectionTestQuery.SendAsync();

        // Assert
        Assert.False(response.IsSuccess, "The query was successful.");
        Assert.IsType<NoHandlerResult>(response.Error);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(0, 2)]
    public async Task MultipleAttributes_Successful_ReturnsValue2(int value1, int value2)
    {
        // Arrange
        TestHandlerSelectionAttribute.Values = new[] { value1, value2 };

        // Act
        var response = await MultipleHandlerSelectionTestQuery.SendAsync();

        // Assert
        Assert.True(response.IsSuccess, "The query was not successful.");
        Assert.Equal(value2, response.Result);
    }
    [Theory]
    [InlineData(0, 3)]
    [InlineData(1, 2)]
    public async Task MultipleAttributes_Failing_ReturnsNoHandlerError(int value1, int value2)
    {
        // Arrange
        TestHandlerSelectionAttribute.Values = new[] { value1, value2 };

        // Act
        var response = await MultipleHandlerSelectionTestQuery.SendAsync();

        // Assert
        Assert.False(response.IsSuccess, "The query was successful.");
        Assert.IsType<NoHandlerResult>(response.Error);
    }
}