using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Rappd.Data.Bson;

public static class BsonInterfaceRegistrator
{
    public static void RegisterInterfaces()
    {
        foreach (var interfaceImplementation in KnownTypesRegistry.Instance.GetInterfaceImplementations())
        {
            var serializer = typeof(ImpliedImplementationInterfaceSerializer<,>).MakeGenericType(interfaceImplementation.@interface, interfaceImplementation.implementation);
            BsonSerializer.RegisterSerializer(interfaceImplementation.@interface, Activator.CreateInstance(serializer) as IBsonSerializer);
        }

        foreach (var baseType in KnownTypesRegistry.Instance.GetBaseTypes())
        {
            var serializer = typeof(BsonPolimorphicInterfaceSerializer<>).MakeGenericType(baseType);
            BsonSerializer.RegisterSerializer(baseType, Activator.CreateInstance(serializer) as IBsonSerializer);
        }
    }

    public static void RegisterInterfaceMap<T>(Action<BsonClassMap> initializer)
    {
        var pack = new ConventionPack();
        pack.AddClassMapConvention($"{typeof(T).Name}InterfaceClassMapConvention", initializer);
        ConventionRegistry.Register($"{typeof(T).Name}InterfaceConventions", pack, t => typeof(T).IsAssignableFrom(t));
    }
}
