using static Rappd.Api.Manifest.ApiManifest;

namespace Rappd.Api.Manifest;

public abstract class ApiManifest(ApiScope[] allScopes, ApiEndpoint[] allEndpoints)
{
    public ApiScope[] AllScopes { get; } = allScopes;
    public ApiEndpoint[] AllEndpoints { get; } = allEndpoints;

    public abstract class ApiScopes<TSelf>
        where TSelf : ApiScopes<TSelf>
    {
        private static Dictionary<string, ApiScope> _allScopes = [];

        public ApiScope[] AllScopes => [.. _allScopes.Values];

        protected static ApiScope Create(string value)
        {
            var scope = new ApiScope(value);
            _allScopes.TryAdd(scope.ToString(), scope);
            return scope;
        }
    }
    public abstract class ApiEndpoints<TSelf>
        where TSelf : ApiEndpoints<TSelf>
    {
        private static Dictionary<string, ApiEndpoint> _allEndpoints = [];

        public ApiEndpoint[] AllEndpoints => [.. _allEndpoints.Values];

        protected static ApiEndpoint Register(ApiEndpoint endpoint)
        {
            _allEndpoints.TryAdd(endpoint.ToString(), endpoint);
            return endpoint;
        }

        protected static ApiEndpoint Get(string path)
            => Register(new(HttpMethod.Get, path));
        protected static ApiEndpoint Post(string path)
            => Register(new(HttpMethod.Post, path));
        protected static ApiEndpoint Put(string path)
            => Register(new(HttpMethod.Put, path));
        protected static ApiEndpoint Patch(string path)
            => Register(new(HttpMethod.Patch, path));
        protected static ApiEndpoint Delete(string path)
            => Register(new(HttpMethod.Delete, path));
        protected static ApiEndpoint Head(string path)
            => Register(new(HttpMethod.Head, path));
        protected static ApiEndpoint Query(string path)
            => Register(new(HttpMethod.Query, path));
        protected static ApiEndpoint Options(string path)
            => Register(new(HttpMethod.Options, path));
        protected static ApiEndpoint Connect(string path)
            => Register(new(HttpMethod.Connect, path));
        protected static ApiEndpoint Trace(string path)
            => Register(new(HttpMethod.Trace, path));
    }
}

public abstract class ApiManifest<TScopes, TEndpoints>() : ApiManifest(Scopes.AllScopes, Endpoints.AllEndpoints)
    where TScopes : ApiScopes<TScopes>, new()
    where TEndpoints : ApiEndpoints<TEndpoints>, new()
{
    public static TScopes Scopes { get; } = new();
    public static TEndpoints Endpoints { get; } = new();
}