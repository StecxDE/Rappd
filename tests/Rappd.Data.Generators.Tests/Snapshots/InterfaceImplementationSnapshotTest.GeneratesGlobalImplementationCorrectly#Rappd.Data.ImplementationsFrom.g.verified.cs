//HintName: Rappd.Data.ImplementationsFrom.g.cs
#nullable enable

namespace Rappd.Data.InterfaceImplementations
{

    internal partial record ITestInterfaceImplementation : ITestInterface
    {
        public string Name { get; init; }
        public void TestMethod()
            => throw new NotImplementedException();
        public void TestMethod2<TTy>(object)
            => throw new NotImplementedException();
    }

}
