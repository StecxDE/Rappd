//HintName: ITestInterfaceImplementation.Generator.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class Implementations
    {
        public static ITestInterface Create<TInterface>(Rappd.Data.InterfaceImplementations.ITestInterfaceImplementation implementation)
            where TInterface : ITestInterface
        {
            return implementation;
        }
    }
}
