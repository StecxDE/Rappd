//HintName: TestImplementation.Registration.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterTestImplementation()
        {
            Rappd.Data.KnownTypesRegistry.Instance.RegisterImplementation(typeof(Rappd.Data.Tests.ITestInterface),typeof(Rappd.Data.Tests.TestImplementation));
        }
    }
}
