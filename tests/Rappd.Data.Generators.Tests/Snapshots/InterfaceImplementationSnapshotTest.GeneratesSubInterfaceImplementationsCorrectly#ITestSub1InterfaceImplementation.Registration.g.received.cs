//HintName: ITestSub1InterfaceImplementation.Registration.g.cs
namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterITestSub1InterfaceImplementation()
        {
            Rappd.Data.KnownTypesRegistry.Instance.Register(typeof(ITestSub1Interface),typeof(Rappd.Data.InterfaceImplementations.ITestSub1InterfaceImplementation));
        }
    }
}
