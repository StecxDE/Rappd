//HintName: ITestSub1InterfaceImplementation.Generator.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class Implementations
    {
        public static ITestSub1Interface CreateITestSub1Interface(string pSub1Prop)
        => new Rappd.Data.InterfaceImplementations.ITestSub1InterfaceImplementation
        {
            Sub1Prop = pSub1Prop,
        };
    }
}
