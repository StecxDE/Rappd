//HintName: ITestSub1Interface.SubRegistration.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterSubITestSub1Interface()
        {
            Rappd.Data.KnownTypesRegistry.Instance.RegisterSubType(typeof(ITestBaseInterface), "sub1", typeof(ITestSub1Interface));
        }
    }
}
