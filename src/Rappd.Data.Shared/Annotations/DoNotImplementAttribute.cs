using System;

namespace Rappd.Data
{
    /// <summary>
    /// Indicates that a interface should not be implemented.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class DoNotImplementAttribute : Attribute
    {
    }
}
