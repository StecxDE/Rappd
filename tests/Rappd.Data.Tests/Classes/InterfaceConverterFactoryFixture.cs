namespace Rappd.Data.Tests;

public class InterfaceConverterFactoryFixture
{
    public InterfaceConverterFactory Factory { get; }

    public InterfaceConverterFactoryFixture()
    {
        Factory = new InterfaceConverterFactory(GetType().Assembly);
    }
}
[CollectionDefinition(nameof(InterfaceConverterFactoryFixtureCollection))]
public class InterfaceConverterFactoryFixtureCollection : ICollectionFixture<InterfaceConverterFactoryFixture> { }