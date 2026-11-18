//HintName: Rappd.Data.Registrations.g.cs
#nullable enable

namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterImplementations()
        {
            Rappd.Data.KnownTypesRegistry.Instance.RegisterConverter<Rappd.Data.Tests.ITestInterface>((implementation)
                => new Rappd.Data.InterfaceImplementations.TestImplementationClosedITestInterfaceImplementation
                {
                    Description = implementation.Description,
                    Name = implementation.Name,
                    Type = implementation.Type,
                }
            );
            Rappd.Data.KnownTypesRegistry.Instance.RegisterImplementation(typeof(Rappd.Data.Tests.ITestInterface),typeof(Rappd.Data.Tests.TestImplementation));
        }
    }
}
