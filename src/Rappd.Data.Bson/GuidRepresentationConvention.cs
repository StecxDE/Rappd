using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Rappd.Data.Bson;

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
