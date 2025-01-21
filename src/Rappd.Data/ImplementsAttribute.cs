namespace Rappd.Data
{
    /// <summary>
    /// Indicates that a class implements a interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public abstract class ImplementsAttribute : Attribute
    {
        /// <summary>
        /// The type of the interface which is implemented by the class.
        /// </summary>
        public Type InterfaceType { get; }
        /// <summary>
        /// Creates a new <see cref="ImplementsAttribute"/> with the given type.
        /// </summary>
        /// <param name="interfaceType">The type of the interface which is implemented by the class.</param>
        internal ImplementsAttribute(Type interfaceType)
            => InterfaceType = interfaceType.IsInterface ? interfaceType : throw new ArgumentException($"The given type '{interfaceType}' is not a interface", nameof(interfaceType));
    }
    /// <summary>
    /// Indicates that a class implements a interface.
    /// </summary>
    /// <typeparam name="TInterface">The type of the interface which is implemented by the class.</typeparam>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ImplementsAttribute<TInterface> : ImplementsAttribute
    {
        /// <summary>
        /// Creates a new <see cref="ImplementsAttribute{TInterface}"/>.
        /// </summary>
        public ImplementsAttribute() : base(typeof(TInterface)) { }
    }
}
