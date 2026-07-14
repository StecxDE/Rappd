//HintName: ITestInterfaceImplementation.Implementation.g.cs
namespace Rappd.Data.InterfaceImplementations
{
    public partial record ITestInterfaceImplementation : ITestInterface
    {
        public System.String Name { get; init; }
        public void TestMethod()
            => throw new NotImplementedException();
        public void TestMethod2<TTy>(System.Object name)
            => throw new NotImplementedException();
    }
}
