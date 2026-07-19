//HintName: ITestSub1InterfaceImplementation.Implementation.g.cs
#nullable enable
namespace Rappd.Data.InterfaceImplementations
{
    public partial record ITestSub1InterfaceImplementation : ITestSub1Interface
    {
        public string Sub1Prop { get; init; }
        public string Type { get;  } = "sub1";
    }
}
