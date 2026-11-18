//HintName: Rappd.Data.Generators.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class Implementations
    {
        public static Rappd.Data.Tests.ITestInterface CreateITestInterface(string pDescription,string pName,string pType)
        => new Rappd.Data.InterfaceImplementations.TestImplementationClosedITestInterfaceImplementation
        {
            Description = pDescription,
            Name = pName,
            Type = pType,
        };
    }
}
