using Rappd.Data.Tests;
using Rappd.Data.Tests.Classes;

[assembly: AssemblyFixture(typeof(InterfaceConverterFactoryFixture))]

namespace Rappd.Data.Tests.Classes;

public class InterfaceConverterFactoryFixture
{
    public InterfaceConverterFactory FactoryNoAdditionalProperties { get; }
    public InterfaceConverterFactory FactoryAdditionalProperties { get; }

    public InterfaceConverterFactoryFixture()
    {
        FactoryNoAdditionalProperties = new InterfaceConverterFactory(false, GetType().Assembly);
        FactoryAdditionalProperties = new InterfaceConverterFactory(true, GetType().Assembly);
    }
}