using System;
using System.Collections.Generic;
using System.Text;

namespace Rappd.Data
{
    /// <summary>
    /// The global registry for known interface types and their implementations.
    /// </summary>
    public class KnownTypesRegistry
    {
        /// <summary>
        /// The current singleton instance of the registry.
        /// </summary>
        public static KnownTypesRegistry Instance { get; } = new KnownTypesRegistry();
        /// <summary>
        /// Private constructor.
        /// </summary>
        private KnownTypesRegistry() { }

        /// <summary>
        /// The internal store for all currently known interface types and their corresponding implementation type.
        /// </summary>
        private Dictionary<Type, Type> _knownTypes = new Dictionary<Type, Type>();

        /// <summary>
        /// Registers a known type.
        /// </summary>
        /// <param name="interfaceType">The type of the interface.</param>
        /// <param name="implementationType">The type of the implementation.</param>
        public void Register(Type interfaceType, Type implementationType)
        {
            // Check if we don't know the interface already and the given type implements the interface
            if (!_knownTypes.ContainsKey(interfaceType) && interfaceType.IsAssignableFrom(implementationType))
                _knownTypes.Add(interfaceType, implementationType);
        }

        /// <summary>
        /// Checks if the given interface type is known.
        /// </summary>
        /// <param name="interfaceType">The interface type.</param>
        /// <returns><see cref="true"/> if the interface type is known, otherwise <see cref="false"/>.</returns>
        public bool IsInterfaceKnown(Type interfaceType)
            => _knownTypes.ContainsKey(interfaceType);

        /// <summary>
        /// Tries to get the implementation type of the given interface type.
        /// </summary>
        /// <param name="interfaceType">The interface type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns><see cref="true"/> if the implementation type is found, otherwise <see cref="false"/>.</returns>
        public bool TryGetImplementationType(Type interfaceType, out Type implementationType)
            => _knownTypes.TryGetValue(interfaceType, out implementationType);
    }
}
