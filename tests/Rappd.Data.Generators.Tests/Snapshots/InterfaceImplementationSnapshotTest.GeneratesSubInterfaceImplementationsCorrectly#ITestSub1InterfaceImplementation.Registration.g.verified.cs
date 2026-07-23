//HintName: ITestSub1InterfaceImplementation.Registration.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterITestSub1InterfaceImplementation()
        {
            Rappd.Data.KnownTypesRegistry.Instance.RegisterImplementation(typeof(ITestSub1Interface),typeof(Rappd.Data.InterfaceImplementations.ITestSub1InterfaceImplementation));
            Rappd.Data.KnownTypesRegistry.Instance.RegisterConverter<ITestSub1Interface>((implementation)
                => new Rappd.Data.InterfaceImplementations.ITestSub1InterfaceImplementation
                {
                    Sub1Prop = implementation.Sub1Prop,
                }
            );
        }
    }
}
