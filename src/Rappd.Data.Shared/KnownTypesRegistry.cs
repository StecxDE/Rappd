using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        /// The internal store for all currently known base types and their discriminator property name and type.
        /// </summary>
        private readonly ConcurrentDictionary<Type, (Type type, string name)> _knownBaseTypes = new ConcurrentDictionary<Type, (Type type, string name)>();
        /// <summary>
        /// The internal store for all currently known base types and their sub types.
        /// </summary>
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Type>> _knownSubTypes = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Type>>();
        /// <summary>
        /// The internal store for all currently known base types and their sub types.
        /// </summary>
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>> _knownDiscriminators = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>>();
        /// <summary>
        /// The internal store for all currently known interface types and their corresponding implementation type.
        /// </summary>
        private readonly ConcurrentDictionary<Type, Type> _knownImplementationTypes = new ConcurrentDictionary<Type, Type>();
        /// <summary>
        /// The internal store for all currently knwon type converters
        /// </summary>
        private readonly static ConcurrentDictionary<Type, Converter> _knownConverters = new ConcurrentDictionary<Type, Converter>();

        public (Type @interface, Type implementation)[] GetInterfaceImplementations()
            => _knownImplementationTypes.Select(kvp => (kvp.Key, kvp.Value)).ToArray();
        public Type[] GetBaseTypes()
            => _knownBaseTypes.Keys.ToArray();

        public void RegisterBaseType(Type baseType, (Type type, string name) discriminator)
        {
            _knownBaseTypes.TryAdd(baseType, discriminator);
            _knownSubTypes.GetOrAdd(baseType, _ => new ConcurrentDictionary<string, Type>());
        }
        public bool IsBaseTypeKnown(Type baseType)
            => _knownBaseTypes.ContainsKey(baseType);
        public bool TryGetDiscriminator(Type baseType, [NotNullWhen(true)] out (Type type, string name) discriminator)
            => _knownBaseTypes.TryGetValue(baseType, out discriminator);
        public void RegisterSubType(Type baseType, object discriminator, Type subType)
        {
            if (!baseType.IsAssignableFrom(subType))
                return;

            var subTypes = _knownSubTypes.GetOrAdd(baseType, _ => new ConcurrentDictionary<string, Type>());
            var discriminatorString = discriminator.ToString();
            subTypes.TryAdd(discriminatorString, subType);

            var discriminators = _knownDiscriminators.GetOrAdd(baseType, _ => new ConcurrentDictionary<Type, object>());
            discriminators.TryAdd(subType, discriminator);
        }
        public bool TryGetSubType(Type baseType, object discriminator, [NotNullWhen(true)] out Type? subType)
        {
            subType = null;
            var discriminatorString = discriminator.ToString();
            return _knownSubTypes.TryGetValue(baseType, out var subTypes) && subTypes.TryGetValue(discriminatorString, out subType);
        }
        public bool TryGetSubTypeDiscriminator(Type baseType, Type subType, out object? discriminator)
        {
            discriminator = null;
            return _knownDiscriminators.TryGetValue(baseType, out var discriminators) && discriminators.TryGetValue(subType, out discriminator);
        }
        public bool TryGetSubTypes(Type baseType, [NotNullWhen(true)] out Type[]? subTypes)
        {
            subTypes = null;
            if(_knownSubTypes.TryGetValue(baseType, out var subTypesDict))
                subTypes = [.. subTypesDict.Values];
            return subTypes is not null;
        }

        /// <summary>
        /// Registers a implementation type.
        /// </summary>
        /// <param name="interfaceType">The type of the interface.</param>
        /// <param name="implementationType">The type of the implementation.</param>
        public void RegisterImplementation(Type interfaceType, Type implementationType)
        {
            // Check if the given type implements the interface
            if (interfaceType.IsAssignableFrom(implementationType))
            {
                // Upsert the implementation type for the interface
                _knownImplementationTypes.TryAdd(interfaceType, implementationType);
            }
        }

        /// <summary>
        /// Registers a converter.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface the converter is for.</typeparam>
        /// <param name="converter">The converter function.</param>
        public void RegisterConverter<TInterface>(Func<TInterface, TInterface> converter)
        {
            var interfaceType = typeof(TInterface);
            _knownConverters.TryAdd(interfaceType, new Converter.Typed<TInterface>(converter));
        }

        /// <summary>
        /// Tries to get the converter for the given interface type.
        /// </summary>
        /// <param name="interfaceType">The type of the interface the converter is for.</param>
        /// <param name="converter">The converter function.</param>
        /// <returns><see cref="true"/> if the converter is found, otherwise <see cref="false"/>.</returns>
        public bool TryGetConverter<TInterface>([NotNullWhen(true)] out Func<TInterface, TInterface>? converter)
        {
            if (_knownConverters.TryGetValue(typeof(TInterface), out var @internal) && @internal is Converter.Typed<TInterface> typed)
            {
                converter = typed.F;
                return true;
            }
            converter = null;
            return false;
        }

        /// <summary>
        /// Checks if the given interface type is known.
        /// </summary>
        /// <param name="interfaceType">The interface type.</param>
        /// <returns><see cref="true"/> if the interface type is known, otherwise <see cref="false"/>.</returns>
        public bool IsInterfaceKnown(Type interfaceType)
            => _knownImplementationTypes.ContainsKey(interfaceType);

        /// <summary>
        /// Tries to get the implementation type of the given interface type.
        /// </summary>
        /// <param name="interfaceType">The interface type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns><see cref="true"/> if the implementation type is found, otherwise <see cref="false"/>.</returns>
        public bool TryGetImplementation(Type interfaceType, [NotNullWhen(true)] out Type? implementationType)
            => _knownImplementationTypes.TryGetValue(interfaceType, out implementationType);

        private abstract class Converter
        {
            internal sealed class Typed<T>(Func<T, T> f) : Converter
            {
                public Func<T, T> F { get; } = f;
            }
        }
    }
}
