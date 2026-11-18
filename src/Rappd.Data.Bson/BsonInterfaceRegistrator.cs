using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;

namespace Rappd.Data.Bson;

internal class BsonPolimorphicInterfaceSerializer<TBase> : IBsonSerializer<TBase>, IBsonDocumentSerializer, IHasDiscriminatorConvention
{
    public Type ValueType { get; } = typeof(TBase);
    public (Type type, string name) DiscriminatorProperty { get; }
    public IDiscriminatorConvention DiscriminatorConvention { get; }

    public BsonPolimorphicInterfaceSerializer()
    {
        if (!KnownTypesRegistry.Instance.TryGetDiscriminator(typeof(TBase), out var discriminator))
            throw new InvalidOperationException($"No discriminator registered for base type {typeof(TBase)}.");
        DiscriminatorProperty = discriminator;
        DiscriminatorConvention = new DiscriminatorConventionImpl(discriminator.name);
    }

    public TBase Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args)
    {
        var document = BsonSerializer.Deserialize<BsonDocument>(context.Reader);
        var discriminator = document.GetElement(DiscriminatorProperty.name);
        var discriminatorValue = discriminator.Value.ToString() ?? "";
        document.RemoveElement(discriminator);
        if (KnownTypesRegistry.Instance.TryGetSubType(typeof(TBase), discriminatorValue, out var subType))
            return (TBase)BsonSerializer.Deserialize(document, subType);
        else
            throw new NotSupportedException();
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        TBase? value)
    {
        if (typeof(TBase).GetProperty(DiscriminatorProperty.name)?.GetValue(value) is object discriminatorValue && KnownTypesRegistry.Instance.TryGetSubType(typeof(TBase), discriminatorValue, out var subType))
        {
            BsonDocument document = value.ToBsonDocument(subType);
            document.Add(new BsonElement(DiscriminatorProperty.name, BsonValue.Create(discriminatorValue)));
            BsonSerializer.Serialize(context.Writer, typeof(BsonDocument), document);
        }
        else
            throw new NotSupportedException();
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value)
        => Serialize(context, args, (TBase)value);

    object? IBsonSerializer.Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args)
        => Deserialize(context, args);

    public bool TryGetMemberSerializationInfo(string memberName, out BsonSerializationInfo? serializationInfo)
    {
        static BsonMemberMap MapMember(BsonClassMap classMap, PropertyInfo property)
        {
            var memberMap = classMap.MapMember(property);
            foreach (var convention in classMap.ConventionPack.Conventions.OfType<IMemberMapConvention>())
                convention.Apply(memberMap);
            return memberMap;
        }

        var property = ValueType.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            ?? ValueType.GetInterfaces()
                .Select(i => i.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance))
                .FirstOrDefault(p => p is not null);

        if (property is not null)
        {
            var classMap = new BsonClassMap(ValueType);
            classMap.AutoMap();
            var memberMap = MapMember(classMap, property);
            classMap.Freeze();
            serializationInfo = new BsonSerializationInfo(memberMap.ElementName, memberMap.GetSerializer(), memberMap.MemberType);
        }
        else
            serializationInfo = null;

        return serializationInfo is not null;
    }


    private class DiscriminatorConventionImpl(string elementName) : StandardDiscriminatorConvention(elementName), IHierarchicalDiscriminatorConvention
    {
        public override BsonValue? GetDiscriminator(Type nominalType, Type actualType)
        {
            if (KnownTypesRegistry.Instance.TryGetSubTypeDiscriminator(nominalType, actualType, out var discriminator))
                return BsonValue.Create(discriminator);
            return null;
        }
    }
}

public class GuidRepresentationConvention : ConventionBase, IMemberMapConvention, IConvention
{
    private readonly BsonType _representation;
    private readonly bool _topLevelOnly;

    public BsonType Representation => _representation;
    public bool TopLevelOnly => _topLevelOnly;

    public GuidRepresentationConvention(BsonType representation)
        : this(representation, topLevelOnly: false)
    {
    }

    public GuidRepresentationConvention(BsonType representation, bool topLevelOnly)
    {
        EnsureRepresentationIsValidForGuids(representation);
        _representation = representation;
        _topLevelOnly = topLevelOnly;
    }

    public void Apply(BsonMemberMap memberMap)
    {
        var memberType = memberMap.MemberType;
        if (!CouldApply(memberType))
            return;
        var serializer = memberMap.GetSerializer();
        var bsonSerializer = ((_topLevelOnly && !IsNullableGuid(serializer.ValueType)) ? Reconfigure(serializer, _representation) : ReconfigureSerializerRecursively(serializer, Reconfigure, _representation));
        if (bsonSerializer != null)
            memberMap.SetSerializer(bsonSerializer);
    }

    private static bool IsNullableGuid(Type t)
        => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(t) == typeof(Guid);
    private static bool CouldApply(Type? type)
    {
        HashSet<Type> visited = [];
        return CouldApplyCore(type);
        bool CouldApplyCore(Type? t)
        {
            if (t is null || !visited.Add(t))
                return false;
            return t == typeof(Guid) ||
               IsNullableGuid(t) ||
               (t.IsArray && CouldApplyCore(t.GetElementType())) ||
               t.GetInterfaces().Prepend(t).Any(i => (
               i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>) && CouldApplyCore(i.GetGenericArguments()[0])) ||
               (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>) && (CouldApplyCore(i.GetGenericArguments()[0]) || CouldApplyCore(i.GetGenericArguments()[1])))
            );
        }
    }

    private static IBsonSerializer? Reconfigure(IBsonSerializer? s, BsonType representation)
        => s?.ValueType == typeof(Guid) ? (s as IRepresentationConfigurable)?.WithRepresentation(representation) : null;
    private static IBsonSerializer? ReconfigureSerializerRecursively(IBsonSerializer? serializer, Func<IBsonSerializer?, BsonType, IBsonSerializer?> reconfigure, BsonType representation)
    {
        if (serializer is not IMultipleChildSerializersConfigurable multipleChildSerializersConfigurable)
        {
            if (serializer is IChildSerializerConfigurable childSerializerConfigurable)
            {
                var bsonSerializer = ReconfigureSerializerRecursively(childSerializerConfigurable.ChildSerializer, reconfigure, representation);
                if (bsonSerializer == null)
                    return null;
                return childSerializerConfigurable.WithChildSerializer(bsonSerializer);
            }

            return reconfigure(serializer, representation);
        }

        var flag = false;
        List<IBsonSerializer> list = [];
        var childSerializers = multipleChildSerializersConfigurable.ChildSerializers;
        foreach (IBsonSerializer bsonSerializer2 in childSerializers)
        {
            var bsonSerializer3 = ReconfigureSerializerRecursively(bsonSerializer2, reconfigure, representation);
            flag = flag || bsonSerializer3 != null;
            list.Add(bsonSerializer3 ?? bsonSerializer2);
        }

        if (!flag)
            return null;

        return multipleChildSerializersConfigurable.WithChildSerializers([.. list]);
    }
    private static void EnsureRepresentationIsValidForGuids(BsonType representation)
    {
        switch (representation)
        {
            case BsonType.ObjectId:
            case BsonType.String:
                return;
            default:
                throw new ArgumentException("Guids can only be represented as String or ObjectId", nameof(representation));
        }
    }
}

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
