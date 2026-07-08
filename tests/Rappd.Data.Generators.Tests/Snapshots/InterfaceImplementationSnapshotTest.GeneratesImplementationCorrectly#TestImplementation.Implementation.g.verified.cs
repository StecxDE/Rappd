//HintName: TestImplementation.Implementation.g.cs
namespace Rappd.Data.Tests
{
    public partial record TestImplementation : Rappd.Data.Tests.ITestInterface
    {
        public System.String Name { get; init; }
        public void TestMethod()
            => throw new NotImplementedException();
        public void TestMethod2<TTy>(System.Object name)
            => throw new NotImplementedException();
    }
}
