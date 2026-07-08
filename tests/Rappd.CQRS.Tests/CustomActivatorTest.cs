using Rappd.CQRS.Tests.Classes;
using System.Reflection;

namespace Rappd.CQRS.Tests;

public class CustomActivatorTest
{
    private CqrsProviderFixture _cqrsProviderFixture;

    public CustomActivatorTest(CqrsProviderFixture cqrsProviderFixture)
        => _cqrsProviderFixture = cqrsProviderFixture;

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task ConstructorInjection_Successful_ReturnsValue(int value)
    {
        // Arrange
        _cqrsProviderFixture.Lock();
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
        var response = await CustomActivatorTestQuery.SendAsync(TestContext.Current.CancellationToken);

        // Cleanup
        CqrsProvider.Configure(Array.Empty<Assembly>(), t => Activator.CreateInstance(t));
        _cqrsProviderFixture.Release();

        // Assert
        Assert.True(response.IsSuccess, "The query was not successful.");
        Assert.Equal(value, response.Result);
    }

    [Fact]
    public async Task ConstructorInjection_Failing_ReturnsValue()
    {
        // Arrange, Act, Assert
        await Assert.ThrowsAsync<HandlerActivationException>(() => CustomActivatorTestQuery.SendAsync(TestContext.Current.CancellationToken));
    }
}