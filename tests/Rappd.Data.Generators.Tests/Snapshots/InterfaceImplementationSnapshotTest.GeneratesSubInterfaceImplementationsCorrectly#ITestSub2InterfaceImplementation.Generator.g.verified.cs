//HintName: ITestSub2InterfaceImplementation.Generator.g.cs
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
    }
}
