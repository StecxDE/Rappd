using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly Dictionary<Type, (Type type, string name)> _knownBaseTypes = [];
        /// <summary>
        /// The internal store for all currently known base types and their sub types.
        /// </summary>
        private readonly Dictionary<Type, Dictionary<object, Type>> _knownSubTypes = [];
        /// <summary>
        /// The internal store for all currently known interface types and their corresponding implementation type.
        /// </summary>
        private readonly Dictionary<Type, Type> _knownImplementationTypes = [];
        /// <summary>
        /// The internal store for all currently knwon type converters
        /// </summary>
        private readonly static Dictionary<Type, Converter> _knownConverters = [];

        public (Type @interface, Type implementation)[] GetInterfaceImplementations()
            => [.._knownImplementationTypes.Select(kvp => (kvp.Key, kvp.Value))];
        public (Type @base, string discriminatorProperty, Type discriminatorType, (object discriminator, Type subtype)[] subtypes)[] GetPolymorphicTypes()
        {
            var list = new List<(Type @base, string discriminatorProperty, Type discriminatorType, (object discriminator, Type subtype)[] subtypes)>();

            foreach (var baseTypeInfo in _knownBaseTypes)
            {
                var @base = baseTypeInfo.Key;
                if(_knownSubTypes.TryGetValue(@base, out var subTypeInfos))
                {
                    var subtypes = new List<(object discriminator, Type subtype)>();
                    foreach (var subTypeInfo in subTypeInfos)
                        subtypes.Add((subTypeInfo.Key, subTypeInfo.Value));
                    list.Add((@base, baseTypeInfo.Value.name, baseTypeInfo.Value.type, [..subtypes]));
                }
            }

            return [.. list];
        }

        public void RegisterBaseType(Type baseType, (Type type, string name) discriminator)
        {
            if (!_knownBaseTypes.ContainsKey(baseType))
                _knownBaseTypes.Add(baseType, discriminator);
            if (!_knownSubTypes.ContainsKey(baseType))
                _knownSubTypes.Add(baseType, []);
        }
        public bool IsBaseTypeKnown(Type baseType)
            => _knownBaseTypes.ContainsKey(baseType);
        public bool TryGetDiscriminator(Type baseType, [NotNullWhen(true)] out (Type type, string name) discriminator)
            => _knownBaseTypes.TryGetValue(baseType, out discriminator);
        public void RegisterSubType(Type baseType, object discriminator, Type subType)
        {
            if (!baseType.IsAssignableFrom(subType))
                return;

            if (!_knownSubTypes.TryGetValue(baseType, out var subTypes))
            {
                subTypes = [];
                _knownSubTypes.Add(baseType, subTypes);
            }

            if (!subTypes.ContainsKey(discriminator))
                subTypes.Add(discriminator, subType);
        }
        public bool TryGetSubType(Type baseType, object discriminator, [NotNullWhen(true)] out Type? subType)
        {
            subType = null;
            return _knownSubTypes.TryGetValue(baseType, out var subTypes) && subTypes.TryGetValue(discriminator, out subType);
        }

        /// <summary>
        /// Registers a implementation type.
        /// </summary>
        /// <param name="interfaceType">The type of the interface.</param>
        /// <param name="implementationType">The type of the implementation.</param>
        public void RegisterImplementation(Type interfaceType, Type implementationType)
        {
            // Check if we don't know the interface already and the given type implements the interface
            if (!_knownImplementationTypes.ContainsKey(interfaceType) && interfaceType.IsAssignableFrom(implementationType))
                _knownImplementationTypes.Add(interfaceType, implementationType);
        }

        /// <summary>
        /// Registers a converter.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface the converter is for.</typeparam>
        /// <param name="converter">The converter function.</param>
        public void RegisterConverter<TInterface>(Func<TInterface, TInterface> converter)
        {
            var interfaceType = typeof(TInterface);
            if (!_knownConverters.ContainsKey(interfaceType))
                _knownConverters.Add(interfaceType, new Converter.Typed<TInterface>(converter));
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
            else
            {
                converter = null;
                return false;
            }
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
