//HintName: TestImplementation.Implementation.g.cs
#nullable enable
namespace Rappd.Data.Tests
{
    public partial record TestImplementation : Rappd.Data.Tests.ITestInterface
    {
        public string Description { get; init; }
        public void TestMethod()
            => throw new NotImplementedException();
        public void TestMethod2<TTy>(object)
            => throw new NotImplementedException();
        public string Name { get; init; }
        public string Type { get; init; }
    }
}
