//HintName: ITestSub2Interface.SubRegistration.g.cs
#nullable enable
namespace Rappd.Data
{
    internal static partial class KnownTypesRegistrator
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void RegisterSubITestSub2Interface()
        {
            Rappd.Data.KnownTypesRegistry.Instance.RegisterSubType(typeof(ITestBaseInterface), "sub2", typeof(ITestSub2Interface));
        }
    }
}
