using Rappd.Data;
using Rappd.Data.Tests;

namespace Rappd.CQRS.Tests;

[Collection(nameof(InterfaceConverterFactoryFixtureCollection))]
public class InterfaceConverterFactoryTest
{
    private readonly InterfaceConverterFactory _factory;

    public InterfaceConverterFactoryTest(InterfaceConverterFactoryFixture fixture)
    {
        _factory = fixture.Factory;
    }

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