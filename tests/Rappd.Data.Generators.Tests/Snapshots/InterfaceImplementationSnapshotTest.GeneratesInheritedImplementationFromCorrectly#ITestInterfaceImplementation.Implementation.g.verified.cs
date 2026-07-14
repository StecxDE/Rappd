//HintName: ITestInterfaceImplementation.Implementation.g.cs
namespace Rappd.Data.InterfaceImplementations
{
    public partial record ITestInterfaceImplementation : Rappd.Data.Tests.ITestInterface
    {
        public System.String Description { get; init; }
        public void TestMethod()
            => throw new NotImplementedException();
        public void TestMethod2<TTy>(System.Object name)
            => throw new NotImplementedException();
        public System.String Name { get; init; }
    }
}
