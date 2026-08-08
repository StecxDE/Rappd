//HintName: Rappd.Data.RegistrationsFrom.g.cs
#nullable enable

namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterImplementationsFrom()
        {
            Rappd.Data.KnownTypesRegistry.Instance.RegisterImplementation(typeof(ITestSub2Interface),typeof(Rappd.Data.InterfaceImplementations.ITestSub2InterfaceImplementation));
            Rappd.Data.KnownTypesRegistry.Instance.RegisterConverter<ITestSub2Interface>((implementation)
                => new Rappd.Data.InterfaceImplementations.ITestSub2InterfaceImplementation
                {
                    Sub2Prop = implementation.Sub2Prop,
                }
            );
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
