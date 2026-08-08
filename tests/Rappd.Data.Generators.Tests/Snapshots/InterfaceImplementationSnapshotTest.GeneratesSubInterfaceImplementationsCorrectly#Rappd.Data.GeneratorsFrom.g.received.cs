//HintName: Rappd.Data.GeneratorsFrom.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class Implementations
    {
        public static ITestSub2Interface CreateITestSub2Interface(string pSub2Prop)
        => new Rappd.Data.InterfaceImplementations.ITestSub2InterfaceImplementation
        {
            Sub2Prop = pSub2Prop,
        };
        public static ITestSub1Interface CreateITestSub1Interface(string pSub1Prop)
        => new Rappd.Data.InterfaceImplementations.ITestSub1InterfaceImplementation
        {
            Sub1Prop = pSub1Prop,
        };
    }
}
