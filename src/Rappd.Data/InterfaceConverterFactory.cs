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
        /// The internal store for all currently known interface types and their corresponding converter type.
        /// </summary>
        private Dictionary<Type, Type> _knownTypes = new Dictionary<Type, Type>();
        /// <summary>
        /// The internal store for all currently known interface types and their corresponding implementation type.
        /// </summary>
        private Dictionary<Type, Type> _knownTypes2 = new Dictionary<Type, Type>();

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
            foreach (var knownType in assemblies.SelectMany(a => a.GetTypes().Select(t => (t.GetCustomAttribute(typeof(ImplementsAttribute<>)), t))))
            {
                // Get the foud implements attribute
                if (knownType.Item1 is ImplementsAttribute attribute)
                {
                    var interfaceType = attribute.InterfaceType;
                    var implementationType = knownType.t;

                    // Check if we don't know the interface already and the given type implements the interface
                    if (!_knownTypes.ContainsKey(interfaceType) && implementationType.IsAssignableTo(interfaceType))
                        _knownTypes.Add(interfaceType, typeof(InterfaceConverter<,>).MakeGenericType(interfaceType, implementationType));
                    // Check if we don't know the interface already and the given type implements the interface
                    if (!_knownTypes2.ContainsKey(interfaceType) && implementationType.IsAssignableTo(interfaceType))
                        _knownTypes2.Add(interfaceType, implementationType);
                }
            }
        }

        /// <summary>
        /// Determines whether the given type can be converted.
        /// </summary>
        /// <param name="typeToConvert">The type is checked as to whether it can be converted.</param>
        /// <returns>True if the type can be converted, false otherwise.</returns>
        public override bool CanConvert(Type typeToConvert)
            => _knownTypes.ContainsKey(typeToConvert) && _knownTypes2.ContainsKey(typeToConvert);
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
            // Check if we don't know the interface
            if (!_knownTypes.TryGetValue(typeToConvert, out var converterType) || !_knownTypes2.TryGetValue(typeToConvert, out var implementationType))
                return null;

            // Create the converter for the interface
            return Activator.CreateInstance(converterType, [EnableAdditionalProperties]) as JsonConverter;
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
                        var interfaceProperties = typeof(TInterface).GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                        var properties = implementationProperties.Where(p => interfaceProperties.Any(ip => ip.Name == p.Name));

                        foreach (var property in properties)
                        {
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
                    var properties = typeof(TInterface).GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (var property in properties)
                    {
                        writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name);
                        JsonSerializer.Serialize(writer, property.GetValue(value), property.PropertyType, options);
                    }
                    writer.WriteEndObject();
                }
            }
        }
    }
}
