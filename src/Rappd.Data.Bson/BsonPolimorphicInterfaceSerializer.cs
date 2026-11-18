using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Reflection;

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
