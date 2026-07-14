//HintName: ITestSub2InterfaceImplementation.Registration.g.cs
namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterITestSub2InterfaceImplementation()
        {
            Rappd.Data.KnownTypesRegistry.Instance.Register(typeof(ITestSub2Interface),typeof(Rappd.Data.InterfaceImplementations.ITestSub2InterfaceImplementation));
        }
    }
}
