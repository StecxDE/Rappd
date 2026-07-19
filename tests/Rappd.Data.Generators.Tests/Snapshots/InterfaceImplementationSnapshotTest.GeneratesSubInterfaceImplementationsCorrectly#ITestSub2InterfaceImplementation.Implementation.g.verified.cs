//HintName: ITestSub2InterfaceImplementation.Implementation.g.cs
#nullable enable
namespace Rappd.Data.InterfaceImplementations
{
    public partial record ITestSub2InterfaceImplementation : ITestSub2Interface
    {
        public string Sub2Prop { get; init; }
        public string Type { get;  } = "sub2";
    }
}
