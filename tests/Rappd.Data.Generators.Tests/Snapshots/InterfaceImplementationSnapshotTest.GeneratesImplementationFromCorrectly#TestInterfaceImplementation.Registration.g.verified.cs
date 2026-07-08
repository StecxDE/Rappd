//HintName: TestInterfaceImplementation.Registration.g.cs
namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterTestInterfaceImplementation()
        {
            Rappd.Data.KnownTypesRegistry.Instance.Register(typeof(Rappd.Data.Tests.ITestInterface),typeof(Rappd.Data.Implementations.TestInterfaceImplementation));
        }
    }
}
