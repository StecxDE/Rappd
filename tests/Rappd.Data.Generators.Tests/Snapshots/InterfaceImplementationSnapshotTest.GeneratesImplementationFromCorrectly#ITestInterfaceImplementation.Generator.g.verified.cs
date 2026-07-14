//HintName: ITestInterfaceImplementation.Generator.g.cs
namespace Rappd.Data
{
    internal static partial class Implementations
    {
        public static Rappd.Data.Tests.ITestInterface Create<TInterface>(Rappd.Data.InterfaceImplementations.ITestInterfaceImplementation implementation)
            where TInterface : Rappd.Data.Tests.ITestInterface
        {
            return implementation;
        }
    }
}
