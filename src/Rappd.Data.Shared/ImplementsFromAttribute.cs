using System;
using System.Collections.Generic;
using System.Text;

namespace Rappd.Data
{
    /// <summary>
    /// Indicates that all interfaces from an assembly are automatically implemented.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public abstract class ImplementsFromAttribute : Attribute
    {
        /// <summary>
        /// The type in the assembly to implement the interfaces from.
        /// </summary>
        public Type AssemblyType { get; }
        /// <summary>
        /// Creates a new <see cref="ImplementsFromAttribute"/> with the given assembly type.
        /// </summary>
        /// <param name="assemblyType">The type in the assembly to implement the interfaces from.</param>
        internal ImplementsFromAttribute(Type assemblyType)
            => AssemblyType = assemblyType;
    }
    /// <summary>
    /// Indicates that all interfaces from an assembly are automatically implemented.
    /// </summary>
    /// <typeparam name="TAssembly">The type in the assembly to implement the interfaces from.</typeparam>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class ImplementsFromAttribute<TAssembly> : ImplementsFromAttribute
    {
        /// <summary>
        /// Creates a new <see cref="ImplementsFromAttribute{TAssembly}"/>.
        /// </summary>
        public ImplementsFromAttribute() : base(typeof(TAssembly)) { }
    }
}
