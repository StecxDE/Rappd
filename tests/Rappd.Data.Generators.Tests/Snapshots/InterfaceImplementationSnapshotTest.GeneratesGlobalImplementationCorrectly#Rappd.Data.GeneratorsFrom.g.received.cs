//HintName: Rappd.Data.GeneratorsFrom.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class Implementations
    {
        public static ITestInterface CreateITestInterface(string pName)
        => new Rappd.Data.InterfaceImplementations.ITestInterfaceImplementation
        {
            Name = pName,
        };
    }
}
