namespace Rappd.Api.Manifest;

public sealed class ApiScope(string name, string value)
{
    public string Name { get; } = name;
    public string Value { get; } = value;

    public override bool Equals(object? obj)
        => obj is ApiScope scope && scope.Value == Value;
    public override int GetHashCode()
        => Value.GetHashCode();
    public override string ToString()
        => Value;
}