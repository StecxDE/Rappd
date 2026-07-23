//HintName: ITestSub2InterfaceImplementation.Registration.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterITestSub2InterfaceImplementation()
        {
            Rappd.Data.KnownTypesRegistry.Instance.RegisterImplementation(typeof(ITestSub2Interface),typeof(Rappd.Data.InterfaceImplementations.ITestSub2InterfaceImplementation));
            Rappd.Data.KnownTypesRegistry.Instance.RegisterConverter<ITestSub2Interface>((implementation)
                => new Rappd.Data.InterfaceImplementations.ITestSub2InterfaceImplementation
                {
                    Sub2Prop = implementation.Sub2Prop,
                }
            );
        }
    }
}
