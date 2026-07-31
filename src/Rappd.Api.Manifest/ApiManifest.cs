using static Rappd.Api.Manifest.ApiManifest;

namespace Rappd.Api.Manifest;

public abstract class ApiManifest(ApiScope[] allScopes, ApiEndpoint[] allEndpoints)
{
    public ApiScope[] AllScopes { get; } = allScopes;
    public ApiEndpoint[] AllEndpoints { get; } = allEndpoints;

    public abstract class ApiScopes
    {
        private static List<ApiScope> _allScopes = [];

        public ApiScope[] AllScopes => [.. _allScopes];

        public static ApiScope Create(string value)
        {
            var scope = new ApiScope(value);
            _allScopes.Add(scope);
            return scope;
        }
    }
    public abstract class ApiEndpoints
    {
        private static List<ApiEndpoint> _allEndpoints = [];

        public ApiEndpoint[] AllEndpoints => [.. _allEndpoints];

        private static ApiEndpoint Register(ApiEndpoint endpoint)
        {
            _allEndpoints.Add(endpoint);
            return endpoint;
        }

        public static ApiEndpoint Get(string path)
            => Register(new(HttpMethod.Get, path));
        public static ApiEndpoint Post(string path)
            => Register(new(HttpMethod.Post, path));
        public static ApiEndpoint Put(string path)
            => Register(new(HttpMethod.Put, path));
        public static ApiEndpoint Patch(string path)
            => Register(new(HttpMethod.Patch, path));
        public static ApiEndpoint Delete(string path)
            => Register(new(HttpMethod.Delete, path));
        public static ApiEndpoint Head(string path)
            => Register(new(HttpMethod.Head, path));
        public static ApiEndpoint Query(string path)
            => Register(new(HttpMethod.Query, path));
        public static ApiEndpoint Options(string path)
            => Register(new(HttpMethod.Options, path));
        public static ApiEndpoint Connect(string path)
            => Register(new(HttpMethod.Connect, path));
        public static ApiEndpoint Trace(string path)
            => Register(new(HttpMethod.Trace, path));
    }
}

public abstract class ApiManifest<TScopes, TEndpoints>() : ApiManifest(Scopes.AllScopes, Endpoints.AllEndpoints)
    where TScopes : ApiScopes, new()
    where TEndpoints : ApiEndpoints, new()
{
    public static TScopes Scopes { get; } = new();
    public static TEndpoints Endpoints { get; } = new();
}