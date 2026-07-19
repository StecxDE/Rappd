using System;

namespace Rappd.Data
{
    /// <summary>
    /// Indicates that a interface should is a sub interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public abstract class SubInterfaceAttribute : Attribute
    {
        /// <summary>
        /// The type of the base interface.
        /// </summary>
        public Type InterfaceType { get; }
        /// <summary>
        /// The value of the discriminator property for this subtype.
        /// </summary>
        public object DiscriminatorValue { get; }

        /// <summary>
        /// Creates a new <see cref="SubInterfaceAttribute"/>.
        /// </summary>
        /// <param name="interfaceType">The type ofs the base interface.</param>
        /// <param name="discriminatorValue">The value of the discriminator property for this subtype.</param>
        public SubInterfaceAttribute(Type interfaceType, object discriminatorValue)
            => (InterfaceType, DiscriminatorValue) = (interfaceType, discriminatorValue);
    }
    /// <summary>
    /// Indicates that a interface should is a sub interface.
    /// </summary>
    /// <typeparam name="TInterface">The type of the base interface.</typeparam>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class SubInterfaceAttribute<TInterface> : SubInterfaceAttribute
    {
        /// <summary>
        /// Creates a new <see cref="SubInterfaceAttribute{TInterface}"/>.
        /// </summary>
        /// <param name="discriminatorValue">The value of the discriminator property for this subtype.</param>
        public SubInterfaceAttribute(object discriminatorValue) : base(typeof(TInterface), discriminatorValue)
        {
        }
    }
}
