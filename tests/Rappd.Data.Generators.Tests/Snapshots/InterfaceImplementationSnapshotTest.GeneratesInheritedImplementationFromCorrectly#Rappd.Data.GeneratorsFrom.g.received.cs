//HintName: Rappd.Data.GeneratorsFrom.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class Implementations
    {
        public static Rappd.Data.Tests.ITestInterface CreateITestInterface(string pDescription,string pName)
        => new Rappd.Data.InterfaceImplementations.ITestInterfaceImplementation
        {
            Description = pDescription,
            Name = pName,
        };
    }
}
