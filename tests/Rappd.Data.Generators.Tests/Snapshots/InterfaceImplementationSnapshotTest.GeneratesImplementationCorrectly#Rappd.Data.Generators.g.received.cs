//HintName: Rappd.Data.Generators.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class Implementations
    {
        public static Rappd.Data.Tests.ITestInterface CreateITestInterface(string pName)
        => new Rappd.Data.InterfaceImplementations.TestImplementationClosedITestInterfaceImplementation
        {
            Name = pName,
        };
    }
}
