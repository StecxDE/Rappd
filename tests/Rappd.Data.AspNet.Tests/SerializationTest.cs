using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Rappd.Data.AspNet.Tests.Classes;
using System.Text.Json;

namespace Rappd.Data.AspNet.Tests;

public class SerializationTest(TestServerFixture TestServerFixture)
{
    [Fact]
    public async Task SerializeAsInterface()
    {
        // Arrange
        var @object = new TestObject
        {
            Identifier = Guid.NewGuid(),
            Name = "Test",
            Description = "Test",
            Additional = "Additional"
        };
        var client = TestServerFixture.TestServer.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/serialize", @object, TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<TestObject>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Empty(result?.Additional ?? "");
    }

    [Fact]
    public async Task DeserializeAsInterface()
    {
        // Arrange
        var @object = new TestObject
        {
            Identifier = Guid.NewGuid(),
            Name = "Test",
            Description = "Test",
            Additional = "Additional"
        };
        var client = TestServerFixture.TestServer.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/deserialize", @object, TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<bool>(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }
}