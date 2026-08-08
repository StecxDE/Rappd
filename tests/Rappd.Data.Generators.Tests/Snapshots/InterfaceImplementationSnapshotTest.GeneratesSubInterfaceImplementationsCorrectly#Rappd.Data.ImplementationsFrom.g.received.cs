//HintName: Rappd.Data.ImplementationsFrom.g.cs
#nullable enable

namespace Rappd.Data.InterfaceImplementations
{

    internal partial record ITestSub2InterfaceImplementation : ITestSub2Interface
    {
        public string Sub2Prop { get; init; }
        public string Type { get;  } = "sub2";
    }


    internal partial record ITestSub1InterfaceImplementation : ITestSub1Interface
    {
        public string Sub1Prop { get; init; }
        public string Type { get;  } = "sub1";
    }

}
