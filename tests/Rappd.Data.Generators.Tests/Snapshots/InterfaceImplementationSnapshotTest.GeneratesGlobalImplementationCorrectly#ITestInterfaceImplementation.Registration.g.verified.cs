//HintName: ITestInterfaceImplementation.Registration.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterITestInterfaceImplementation()
        {
            Rappd.Data.KnownTypesRegistry.Instance.RegisterImplementation(typeof(ITestInterface),typeof(Rappd.Data.InterfaceImplementations.ITestInterfaceImplementation));
        }
    }
}
