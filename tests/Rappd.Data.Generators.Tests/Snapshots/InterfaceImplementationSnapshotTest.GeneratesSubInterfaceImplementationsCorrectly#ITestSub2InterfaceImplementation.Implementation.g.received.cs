//HintName: ITestSub2InterfaceImplementation.Implementation.g.cs
namespace Rappd.Data.InterfaceImplementations
{
    public partial record ITestSub2InterfaceImplementation : ITestSub2Interface
    {
        public System.String Sub2Prop { get; init; }
        public System.String Type { get; init; } => "sub2";
    }
}
