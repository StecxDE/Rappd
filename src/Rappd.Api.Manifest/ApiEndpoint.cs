using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Rappd.Api.Manifest;

public interface IApiEndpointMetadata
{

}
public sealed class ApiEndpointMetadata<T>(T value) : IApiEndpointMetadata
{
    public T Value { get; } = value;

    public override int GetHashCode()
        => Value?.GetHashCode() ?? 0;
    public override bool Equals(object? obj)
    {
        if (obj is not ApiEndpointMetadata<T> metadata)
            return false;
        if (Value is null && metadata.Value is null)
            return true;
        return Value?.Equals(metadata.Value) ?? false;
    }
    public override string ToString()
        => $"{typeof(ApiEndpointMetadata<T>).Name}[{typeof(T).Name}]({Value})" ?? "";
}

public sealed partial class ApiEndpoint(HttpMethod method, string path)
{
    public HttpMethod Method { get; } = method;
    public string Path { get; } = path;

    public HashSet<string> QueryParameters { get; } = [];
    public List<IApiEndpointMetadata> Metadata { get; } = []; 

    [GeneratedRegex(@"\{[^{}]+\}")]
    private static partial Regex RouteParameterRegex();

    public ApiEndpoint WithQueryParameters(params string[] names)
    {
        foreach (var name in names)
            QueryParameters.Add(name);
        return this;
    }
    public ApiEndpoint WithRequiredScope(ApiScope scope)
    {
        Metadata.Add(new ApiEndpointMetadata<ApiScope>(scope));
        return this;
    }
    public ApiEndpoint WithMetadata(IApiEndpointMetadata metadata)
    {
        Metadata.Add(metadata);
        return this;
    }

    public string Construct((string name, object value)[] queryParameters, params object[] parameters)
    {
        var uri = Construct(parameters);
        return $"{uri}?{string.Join("&", queryParameters.Where(qp => QueryParameters.Contains(qp.name)).Select(qp => $"{qp.name}={qp.value}"))}";
    }
    public string Construct(params object[] parameters)
    {
        var index = 0;
        return RouteParameterRegex().Replace(Path, _ =>
        {
            if (index >= parameters.Length)
                throw new ArgumentException($"Not enough parameters for route '{Path}'");
            return Uri.EscapeDataString(parameters[index++]?.ToString() ?? string.Empty);
        });
    }

    public override string ToString()
        => $"{Method.Method} {Path}";
}