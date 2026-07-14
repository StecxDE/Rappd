//HintName: ITestSub1InterfaceImplementation.Implementation.g.cs
namespace Rappd.Data.InterfaceImplementations
{
    public partial record ITestSub1InterfaceImplementation : ITestSub1Interface
    {
        public System.String Sub1Prop { get; init; }
        public System.String Type { get; init; } => "sub1";
    }
}
