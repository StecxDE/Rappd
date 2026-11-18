//HintName: Rappd.Data.RegistrationsFrom.g.cs
#nullable enable

namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterImplementationsFrom()
        {
            Rappd.Data.KnownTypesRegistry.Instance.RegisterImplementation(typeof(ITestInterface),typeof(Rappd.Data.InterfaceImplementations.ITestInterfaceImplementation));
            Rappd.Data.KnownTypesRegistry.Instance.RegisterConverter<ITestInterface>((implementation)
                => new Rappd.Data.InterfaceImplementations.ITestInterfaceImplementation
                {
                    Name = implementation.Name,
                }
            );
        }
    }
}
