//HintName: Rappd.Data.BaseInterfaceRegistrations.g.cs
#nullable enable

namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterBaseInterfaces()
        {
            Rappd.Data.KnownTypesRegistry.Instance.RegisterBaseType(typeof(ITestBaseInterface),(typeof(System.String),nameof(ITestBaseInterface.Type)));
        }
    }
}
