using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

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

        foreach (var polymorphicType in KnownTypesRegistry.Instance.GetPolymorphicTypes())
        {
            var baseMap = BsonClassMap.LookupClassMap(polymorphicType.@base);
            baseMap.SetDiscriminatorIsRequired(true);
            foreach (var subType in polymorphicType.subtypes)
            {
                baseMap.AddKnownType(subType.subtype);
                var subMap = BsonClassMap.LookupClassMap(subType.subtype);
                subMap.SetDiscriminator(subType.discriminator.ToString());
            }
        }
    }
}
