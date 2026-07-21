using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Rappd.Data
{
    /// <summary>
    /// A converter factory supporting the conversion of interfaces to their implementation types marked with the <see cref="ImplementsAttribute{TInterface}"/> attribute.
    /// </summary>
    public class InterfaceConverterFactory : JsonConverterFactory
    {
        /// <summary>
        /// Enables that properties not contained in the interface should be serialized/deserialized.
        /// </summary>
        public bool EnableAdditionalProperties { get; } = false;

        /// <summary>
        /// Creates a new <see cref="InterfaceConverterFactory"/> with the given <see cref="Assembly"/> array.
        /// </summary>
        /// <param name="enableAdditionalProperties">Enables that properties not contained in the interface should be serialized/deserialized.</param>
        /// <param name="assemblies">The assemblies containing the known implementation types.</param>
        public InterfaceConverterFactory(bool enableAdditionalProperties, params Assembly[] assemblies) : this(assemblies)
        {
            EnableAdditionalProperties = enableAdditionalProperties;
        }
        /// <summary>
        /// Creates a new <see cref="InterfaceConverterFactory"/> with the given <see cref="Assembly"/> array.
        /// </summary>
        /// <param name="assemblies">The assemblies containing the known implementation types.</param>
        public InterfaceConverterFactory(params Assembly[] assemblies)
        {
            // Find all types marked with the implements attribute
            foreach (var knownType in assemblies.SelectMany(a => a.GetTypes().Select(t => (
                t.GetCustomAttribute<ImplementsAttribute>(),
                t.GetCustomAttribute<BaseInterfaceAttribute>(),
                t.GetCustomAttribute<SubInterfaceAttribute>(),
                t
            ))))
            {
                // Get the found implements attribute
                if (knownType.Item1 is ImplementsAttribute implementsAttribute)
                {
                    // Register the types
                    KnownTypesRegistry.Instance.RegisterImplementation(implementsAttribute.InterfaceType, knownType.t);
                }
                // Get the found base attribute
                if (knownType.Item2 is BaseInterfaceAttribute baseAttribute && knownType.t.GetProperty(baseAttribute.DiscriminatorProperty) is PropertyInfo propertyInfo)
                {
                    // Register the type
                    KnownTypesRegistry.Instance.RegisterBaseType(knownType.t, (propertyInfo.PropertyType, propertyInfo.Name));
                }
                // Get the found sub attribute
                if (knownType.Item3 is SubInterfaceAttribute subAttribute)
                {
                    // Register the types
                    KnownTypesRegistry.Instance.RegisterSubType(subAttribute.InterfaceType, subAttribute.DiscriminatorValue, knownType.t);
                }
            }
        }

        /// <summary>
        /// Determines whether the given type can be converted.
        /// </summary>
        /// <param name="typeToConvert">The type is checked as to whether it can be converted.</param>
        /// <returns>True if the type can be converted, false otherwise.</returns>
        public override bool CanConvert(Type typeToConvert)
            => KnownTypesRegistry.Instance.IsInterfaceKnown(typeToConvert) || KnownTypesRegistry.Instance.IsBaseTypeKnown(typeToConvert);
        /// <summary>
        /// Creates a converter for the given <see cref="Type"/>.
        /// </summary>
        /// <param name="typeToConvert">The <see cref="Type"/> being converted.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
        /// <returns>
        /// An instance of a <see cref="JsonConverter{T}"/> where T is compatible with <paramref name="typeToConvert"/>.
        /// If <see langword="null"/> is returned, a <see cref="NotSupportedException"/> will be thrown.
        /// </returns>
        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if we know the interface
            if (KnownTypesRegistry.Instance.TryGetImplementation(typeToConvert, out var implementationType))
            {
                // Create the converter type
                var converterType = typeof(InterfaceConverter<,>).MakeGenericType(typeToConvert, implementationType);

                // Create the converter for the interface
                return Activator.CreateInstance(converterType, [EnableAdditionalProperties]) as JsonConverter;
            }
            // Check if we know the base type
            else if (KnownTypesRegistry.Instance.TryGetDiscriminator(typeToConvert, out var discriminatorType))
            {
                // Create the converter type
                var converterType = typeof(BaseTypeConverter<>).MakeGenericType(typeToConvert);

                // Create the converter for the base type
                return Activator.CreateInstance(converterType, [discriminatorType]) as JsonConverter;
            }
            else
                return null;
        }

        /// <summary>
        /// The converter implementation used to convert interfaces to their implementation type.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface to convert.</typeparam>
        /// <typeparam name="TImplementation">The type to convert the interface to.</typeparam>
        private class InterfaceConverter<TInterface, TImplementation> : JsonConverter<TInterface>
            where TImplementation : TInterface
        {
            /// <summary>
            /// Enables that properties not contained in the interface should be serialized/deserialized.
            /// </summary>
            public bool EnableAdditionalProperties { get; } = false;

            public InterfaceConverter(bool enableAdditionalProperties)
            {
                EnableAdditionalProperties = enableAdditionalProperties;
            }

            public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (EnableAdditionalProperties)
                {
                    // Use the default converter of the implementation type to deserialize the interface
                    var converter = (JsonConverter<TImplementation>)options.GetConverter(typeof(TImplementation));
                    return converter.Read(ref reader, typeof(TImplementation), options);
                }
                else
                {
                    using (var document = JsonDocument.ParseValue(ref reader))
                    {
                        var instance = Activator.CreateInstance<TImplementation>();

                        var implementationProperties = typeof(TImplementation).GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        List<PropertyInfo> interfaceProperties = [];
                        void AddProperties(Type type)
                        {
                            interfaceProperties.AddRange(type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
                            foreach (var @interface in type.GetInterfaces())
                                AddProperties(@interface);
                        }
                        AddProperties(typeof(TInterface));

                        var properties = implementationProperties.Where(p => interfaceProperties.Any(ip => ip.Name == p.Name));

                        foreach (var property in properties)
                        {
                            if (!property.CanWrite)
                                continue;

                            JsonElement jsonProperty;
                            if (options.PropertyNameCaseInsensitive)
                            {
                                jsonProperty = document.RootElement.EnumerateObject().FirstOrDefault(element => element.Name.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase)).Value;
                            }
                            else
                            {
                                var jsonPropertyName = options.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name;
                                document.RootElement.TryGetProperty(jsonPropertyName, out jsonProperty);
                            }

                            var value = jsonProperty.Deserialize(property.PropertyType, options);
                            property.SetValue(instance, value);
                        }

                        return instance;
                    }
                }
            }

            public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
            {
                if (EnableAdditionalProperties)
                {
                    // Use the default converter of the implementation type to serialize the interface
                    var converter = (JsonConverter<TImplementation>)options.GetConverter(typeof(TImplementation));
                    converter.Write(writer, (TImplementation)value!, options);
                }
                else
                {
                    writer.WriteStartObject();
                    List<PropertyInfo> properties = [];
                    void AddProperties(Type type)
                    {
                        properties.AddRange(type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
                        foreach (var @interface in type.GetInterfaces())
                            AddProperties(@interface);
                    }
                    AddProperties(typeof(TInterface));
                    foreach (var property in properties)
                    {
                        writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name);
                        JsonSerializer.Serialize(writer, property.GetValue(value), property.PropertyType, options);
                    }
                    writer.WriteEndObject();
                }
            }
        }

        /// <summary>
        /// The converter implementation used to convert base types to their sub type.
        /// </summary>
        /// <typeparam name="TBaseType">The base type to convert.</typeparam>
        private class BaseTypeConverter<TBaseType> : JsonConverter<TBaseType>
        {
            public (Type type, string name) DiscriminatorProperty { get; }

            public BaseTypeConverter((Type type, string name) discriminatorProperty)
            {
                DiscriminatorProperty = discriminatorProperty;
            }

            public override TBaseType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                using (var document = JsonDocument.ParseValue(ref reader))
                {
                    if (document.RootElement.TryGetProperty(options.PropertyNamingPolicy?.ConvertName(DiscriminatorProperty.name) ?? DiscriminatorProperty.name, out var jsonProperty) && jsonProperty.Deserialize(DiscriminatorProperty.type, options) is object discriminatorValue && KnownTypesRegistry.Instance.TryGetSubType(typeof(TBaseType), discriminatorValue, out var subType))
                    {
                        return (TBaseType?)document.Deserialize(subType, options);
                    }
                    else
                        throw new NotSupportedException();
                }
            }

            public override void Write(Utf8JsonWriter writer, TBaseType value, JsonSerializerOptions options)
            {
                if (typeof(TBaseType).GetProperty(DiscriminatorProperty.name)?.GetValue(value) is object discriminatorValue && KnownTypesRegistry.Instance.TryGetSubType(typeof(TBaseType), discriminatorValue, out var subType))
                {
                    var converterType = typeof(JsonConverter<>).MakeGenericType(subType);
                    var converter = options.GetConverter(subType);
                    converterType.GetMethod(nameof(JsonConverter<>.Write))?.Invoke(converter, [writer, value, options]);
                }
                else
                    throw new NotSupportedException();
            }
        }
    }
}
