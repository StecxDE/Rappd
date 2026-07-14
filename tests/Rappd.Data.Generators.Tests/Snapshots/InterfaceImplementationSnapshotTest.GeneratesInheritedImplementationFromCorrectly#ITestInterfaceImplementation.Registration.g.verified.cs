//HintName: ITestInterfaceImplementation.Registration.g.cs
namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterITestInterfaceImplementation()
        {
            Rappd.Data.KnownTypesRegistry.Instance.Register(typeof(Rappd.Data.Tests.ITestInterface),typeof(Rappd.Data.InterfaceImplementations.ITestInterfaceImplementation));
        }
    }
}
