using Rappd.Data.Tests.Classes;
using System.Text.Json;

namespace Rappd.Data.Tests;

public class InterfaceConverterFactoryTest
{
    private readonly JsonSerializerOptions _optionsN;
    private readonly JsonSerializerOptions _optionsA;

    public InterfaceConverterFactoryTest(InterfaceConverterFactoryFixture fixture)
    {
        _optionsN = new JsonSerializerOptions();
        _optionsN.Converters.Add(fixture.FactoryNoAdditionalProperties);
        _optionsA = new JsonSerializerOptions();
        _optionsA.Converters.Add(fixture.FactoryAdditionalProperties);
    }

    private static string GetJsonPropertyName(string name, JsonSerializerOptions options)
        => options.PropertyNamingPolicy?.ConvertName(name) ?? name;

    #region NoAdditionalProperties

    [Fact]
    public async Task Serialize_NormalClass_NoAdditionalProperties()
    {
        // Arrange
        TestObject @object = new()
        {
            Identifier = Guid.NewGuid(),
            Description = "Test Description",
            Name = "Test Name",
            Additional = "Additional"
        };

        // Act
        var json = JsonSerializer.SerializeToDocument<ITestInterface>(@object, _optionsN);

        // Assert
        Assert.False(json.RootElement.TryGetProperty(nameof(TestObject.Additional), out _), "Serialized additional properties");
    }

    [Fact]
    public async Task Serialize_NestedClass_NoAdditionalProperties()
    {
        // Arrange
        NestedTestObject @object = new()
        {
            Identifier = Guid.NewGuid(),
            Name = "Nested Test Name",
            Nested = new TestObject
            {
                Identifier = Guid.NewGuid(),
                Name = "Test Name",
                Description = "Test Description",
                Additional = "Additional"
            },
            Additional = "Nested Additional"
        };
       
        // Act
        var json = JsonSerializer.SerializeToDocument<INestedTestInterface>(@object, _optionsN);

        // Assert
        Assert.False(json.RootElement.TryGetProperty(GetJsonPropertyName(nameof(NestedTestObject.Additional), _optionsN), out _), "Serialized additional properties");
        Assert.False(json.RootElement.GetProperty(GetJsonPropertyName(nameof(NestedTestObject.Nested), _optionsN)).TryGetProperty(GetJsonPropertyName(nameof(TestObject.Additional), _optionsN), out _), "Serialized additional nested properties");
    }

    [Fact]
    public async Task Deserialize_NormalClass_NoAdditionalProperties()
    {
        // Arrange
        var json = @"{
            ""Identifier"": ""9fc93e52-2b8e-423c-8f82-97005f2e839c"",
            ""Name"": ""Test Name"",
            ""Description"": ""Test Description"",
            ""Additional"": ""Additional""
        }";

        // Act
        var @interface = JsonSerializer.Deserialize<ITestInterface>(json, _optionsN);

        // Assert
        Assert.True(@interface is TestObject { Additional: "" }, "Deserialized additional properties");
    }

    [Fact]
    public async Task Deserialize_NestedClass_NoAdditionalProperties()
    {
        // Arrange
        var json = @"{
            ""Identifier"":""43d716dd-9681-47ff-b23e-89c50cfff32e"",
            ""Name"":""Nested Test Name"",
            ""Nested"": {
                ""Identifier"":""c8b14862-7d7c-4910-be3f-5c4ad9ddb3a2"",
                ""Name"":""Test Name"",
                ""Description"":""Test Description"",
                ""Additional"":""Additional""
            },
            ""Additional"":""Nested Additional""
        }";

        // Act
        var @interface = JsonSerializer.Deserialize<INestedTestInterface>(json, _optionsN);

        // Assert
        Assert.True(@interface is NestedTestObject { Additional: "" }, "Deserialized additional properties");
        Assert.True(@interface.Nested is TestObject { Additional: "" }, "Deserialized additional nested properties");
    }

    #endregion

    #region AdditionalProperties

    [Fact]
    public async Task Serialize_NormalClass_AdditionalProperties()
    {
        // Arrange
        TestObject @object = new()
        {
            Identifier = Guid.NewGuid(),
            Description = "Test Description",
            Name = "Test Name",
            Additional = "Additional"
        };

        // Act
        var json = JsonSerializer.SerializeToDocument<ITestInterface>(@object, _optionsA);

        // Assert
        Assert.True(json.RootElement.TryGetProperty(nameof(TestObject.Additional), out _), "Serialized no additional properties");
    }

    [Fact]
    public async Task Serialize_NestedClass_AdditionalProperties()
    {
        // Arrange
        NestedTestObject @object = new()
        {
            Identifier = Guid.NewGuid(),
            Name = "Nested Test Name",
            Nested = new TestObject
            {
                Identifier = Guid.NewGuid(),
                Name = "Test Name",
                Description = "Test Description",
                Additional = "Additional"
            },
            Additional = "Nested Additional"
        };

        // Act
        var json = JsonSerializer.SerializeToDocument<INestedTestInterface>(@object, _optionsA);

        // Assert
        Assert.True(json.RootElement.TryGetProperty(GetJsonPropertyName(nameof(NestedTestObject.Additional), _optionsA), out _), "Serialized no additional properties");
        Assert.True(json.RootElement.GetProperty(GetJsonPropertyName(nameof(NestedTestObject.Nested), _optionsA)).TryGetProperty(GetJsonPropertyName(nameof(TestObject.Additional), _optionsA), out _), "Serialized no additional nested properties");
    }

    [Fact]
    public async Task Deserialize_NormalClass_AdditionalProperties()
    {
        // Arrange
        var json = @"{
            ""Identifier"": ""9fc93e52-2b8e-423c-8f82-97005f2e839c"",
            ""Name"": ""Test Name"",
            ""Description"": ""Test Description"",
            ""Additional"": ""Additional""
        }";

        // Act
        var @interface = JsonSerializer.Deserialize<ITestInterface>(json, _optionsA);

        // Assert
        Assert.False(@interface is TestObject { Additional: "" }, "Deserialized no additional properties");
    }

    [Fact]
    public async Task Deserialize_NestedClass_AdditionalProperties()
    {
        // Arrange
        var json = @"{
            ""Identifier"":""43d716dd-9681-47ff-b23e-89c50cfff32e"",
            ""Name"":""Nested Test Name"",
            ""Nested"": {
                ""Identifier"":""c8b14862-7d7c-4910-be3f-5c4ad9ddb3a2"",
                ""Name"":""Test Name"",
                ""Description"":""Test Description"",
                ""Additional"":""Additional""
            },
            ""Additional"":""Nested Additional""
        }";

        // Act
        var @interface = JsonSerializer.Deserialize<INestedTestInterface>(json, _optionsA);

        // Assert
        Assert.False(@interface is NestedTestObject { Additional: "" }, "Deserialized no additional properties");
        Assert.False(@interface?.Nested is TestObject { Additional: "" }, "Deserialized no additional nested properties");
    }

    #endregion
}