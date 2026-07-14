//HintName: TestImplementation.Generator.g.cs
namespace Rappd.Data
{
    internal static partial class Implementations
    {
        public static Rappd.Data.Tests.ITestInterface Create<TInterface>(Rappd.Data.Tests.TestImplementation implementation)
            where TInterface : Rappd.Data.Tests.ITestInterface
        {
            return implementation;
        }
    }
}
