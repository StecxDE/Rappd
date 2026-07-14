using System;

namespace Rappd.Data
{
    /// <summary>
    /// Indicates that a interface should is a base interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class BaseInterfaceAttribute : Attribute
    {
        /// <summary>
        /// The name of the property deciding which sub type is used.
        /// </summary>
        public string DiscriminatorProperty { get; }
        /// <summary>
        /// Creates a new <see cref="BaseInterfaceAttribute"/>.
        /// </summary>
        /// <param name="discriminatorProperty">The name of the property deciding which sub type is used.</param>
        public BaseInterfaceAttribute(string discriminatorProperty) 
            => DiscriminatorProperty = discriminatorProperty;
    }
}
