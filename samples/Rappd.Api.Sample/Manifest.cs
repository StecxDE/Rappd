using Rappd.Api.Manifest;
using System.Runtime.InteropServices;

namespace Rappd.Api.Sample;

public sealed class Manifest : ApiManifest<Manifest.ManifestScopes, Manifest.ManifestEndpoints>
{
    public sealed class ManifestScopes : ApiScopes<ManifestScopes>
    {
        public ApiScope Read { get; } = Create(nameof(Read), "sample:read");
        public ApiScope Write { get; } = Create(nameof(Write), "sample:write");
    }

    public sealed class ManifestEndpoints : ApiEndpoints<ManifestEndpoints>
    {
        public ApiEndpoint GetWeather { get; } = Get("/weather")
            .WithRequiredScope(Scopes.Read);
        public ApiEndpoint GetRequest { get; } = Get("/request")
            .WithQueryParameters("type")
            .WithRequiredScope(Scopes.Read);
        public ApiEndpoint PostRequest { get; } = Post("/request")
            .WithRequiredScope(Scopes.Write);
    }
}
