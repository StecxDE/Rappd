using System.Reflection;

namespace Rappd.CQRS.Tests;

public class CustomActivatorTest
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task ConstructorInjection_Successful_ReturnsValue(int value)
    {
        // Arrange
        CqrsProvider.Configure(Array.Empty<Assembly>(), t =>
        {
            if (t == typeof(CustomActivatorTestQueryHandler))
            {
                return new CustomActivatorTestQueryHandler(value);
            }
            else
            {
                return Activator.CreateInstance(t);
            }
        });

        // Act
        var response = await CustomActivatorTestQuery.SendAsync();

        // Cleanup
        CqrsProvider.Configure(Array.Empty<Assembly>(), t => Activator.CreateInstance(t));

        // Assert
        Assert.True(response.IsSuccess, "The query was not successful.");
        Assert.Equal(value, response.Result);
    }

    [Fact]
    public async Task ConstructorInjection_Failing_ReturnsValue()
    {
        // Arrange, Act, Assert
        await Assert.ThrowsAsync<HandlerActivationException>(() => CustomActivatorTestQuery.SendAsync());
    }
}