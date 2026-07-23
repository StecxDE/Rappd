using System;

namespace Rappd.Data
{
    /// <summary>
    /// Indicates the default value of a property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DefaultValueAttribute : Attribute
    {
        /// <summary>
        /// The default value.
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// Creates a new <see cref="DefaultValueAttribute"/>.
        /// </summary>
        /// <param name="value">The default value.</param>
        public DefaultValueAttribute(object? value)
            => Value = value;
    }
}
