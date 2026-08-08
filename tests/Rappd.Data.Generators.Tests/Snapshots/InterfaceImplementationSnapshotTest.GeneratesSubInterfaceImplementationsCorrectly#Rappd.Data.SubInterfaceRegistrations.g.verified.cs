//HintName: Rappd.Data.SubInterfaceRegistrations.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterSubInterfaces()
        {
            Rappd.Data.KnownTypesRegistry.Instance.RegisterSubType(typeof(ITestBaseInterface), "sub1", typeof(ITestSub1Interface));
            Rappd.Data.KnownTypesRegistry.Instance.RegisterSubType(typeof(ITestBaseInterface), "sub2", typeof(ITestSub2Interface));
        }
    }
}
