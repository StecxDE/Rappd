namespace Rappd.Data;

public static class TypeToInterfaceConverter
{
    public static bool TryConvertTo<TInterface>(TInterface type, out TInterface @interface)
    {
        if (type is not null && KnownTypesRegistry.Instance.TryGetConverter<TInterface>(out var converter))
        {
            @interface = converter(type);
            return true;
        }
        else
        {
            @interface = type;
            return false;
        }
    }
}
