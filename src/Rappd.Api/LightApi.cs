using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rappd.Api.Manifest;
using System.Reflection;

namespace Rappd.Api
{
    /// <summary>
    /// A class used to create a api with minimal overhead.
    /// </summary>
    public class LightApi
    {
        /// <summary>
        /// The type of the handler used to initialize the <see cref="LightApi"/> configuration and logging.
        /// </summary>
        /// <param name="hostApplication">The builder used to initialize the hostApplication of the <see cref="LightApi"/>.</param>
        /// <param name="webHost">The builder used to initialize the web host of the <see cref="LightApi"/>.</param>
        public delegate Task InitializationHandler(IHostApplicationBuilder hostApplication, IWebHostBuilder webHost);
        /// <summary>
        /// The type of the handler used to configure the <see cref="LightApi"/> services.
        /// </summary>
        /// <param name="configuration">The configuration of the <see cref="LightApi"/>.</param>
        /// <param name="services">The collection used to configure the services of the <see cref="LightApi"/>.</param>
        public delegate void ConfigurationHandler(IConfiguration configuration, IServiceCollection services);
        /// <summary>
        /// The type of the handler used to build the <see cref="LightApi"/> request pipeline.
        /// </summary>
        /// <param name="app">Provides informations about the <see cref="LightApi"/> application state.</param>
        /// <param name="pipeline">The builder used to build the request pipeline of the <see cref="LightApi"/>.</param>
        public delegate void BuildHandler((IServiceProvider services, IConfiguration configuration, IWebHostEnvironment environment, IHostApplicationLifetime lifetime, ILogger logger, ICollection<string> urls) app, IApplicationBuilder pipeline);
        /// <summary>
        ///  The type of the handler used to map the <see cref="LightApi"/> routes.
        /// </summary>
        /// <param name="configuration">The configuration of the <see cref="LightApi"/>.</param>
        /// <param name="route">The builder used to map the routes of the <see cref="LightApi"/>.</param>
        public delegate void MapHandler(IConfiguration configuration, IEndpointRouteBuilder route);

        /// <summary>
        /// The handler used to initialize the <see cref="LightApi"/> configuration and logging.
        /// </summary>
        private readonly InitializationHandler? _initialize;
        /// <summary>
        /// The handler used to configure the <see cref="LightApi"/> services.
        /// </summary>
        private readonly ConfigurationHandler? _configure;
        /// <summary>
        /// The handler used to build the <see cref="LightApi"/> request pipeline.
        /// </summary>
        private readonly BuildHandler? _build;
        /// <summary>
        /// The handler used to map the <see cref="LightApi"/> routes.
        /// </summary>
        private readonly MapHandler? _map;

        /// <summary>
        /// Creates a new <see cref="LightApi"/> with the specified handlers.
        /// </summary>
        /// <param name="initialize">The handler used to initialize the <see cref="LightApi"/> configuration and logging.</param>
        /// <param name="configure">The handler used to configure the <see cref="LightApi"/> services.</param>
        /// <param name="build">The handler used to build the <see cref="LightApi"/> request pipeline.</param>
        /// <param name="map">The handler used to map the <see cref="LightApi"/> routes.</param>
        public LightApi(InitializationHandler? initialize = null, ConfigurationHandler? configure = null, BuildHandler? build = null, MapHandler? map = null)
            => (_initialize, _configure, _build, _map) = (initialize, configure, build, map);

        protected virtual async Task<WebApplication> BuildAsync(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Initialize if possible
            await (_initialize?.Invoke(builder, builder.WebHost) ?? Task.CompletedTask);
            // Configure if possible
            _configure?.Invoke(builder.Configuration, builder.Services);
            var app = builder.Build();
            // Build if possible
            _build?.Invoke((app.Services, app.Configuration, app.Environment, app.Lifetime, app.Logger, app.Urls), app);
            // Map if possible
            _map?.Invoke(app.Configuration, app);
            return app;
        }

        /// <summary>
        /// Runs the <see cref="LightApi"/> asyncronously.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>A <see cref="Task"/> that represents the entire runtime of the <see cref="LightApi"/> from startup to shutdown.</returns>
        public async Task RunAsync(string[] args)
        {
            var app = await BuildAsync(args);
            await app.RunAsync();
        }
    }

    public enum ApiValidationMode
    {
        Strict,
        Loose,
        None
    }

    public class LightApi<TManifest> : LightApi
        where TManifest : ApiManifest, new()
    {
        private readonly ApiValidationMode _validationMode;

        /// <summary>
        /// Creates a new <see cref="LightApi"/> with the specified handlers.
        /// </summary>
        /// <param name="initialize">The handler used to initialize the <see cref="LightApi"/> configuration and logging.</param>
        /// <param name="configure">The handler used to configure the <see cref="LightApi"/> services.</param>
        /// <param name="build">The handler used to build the <see cref="LightApi"/> request pipeline.</param>
        /// <param name="map">The handler used to map the <see cref="LightApi"/> routes.</param>
        public LightApi(InitializationHandler? initialize = null, ConfigurationHandler? configure = null, BuildHandler? build = null, MapHandler? map = null, ApiValidationMode validationMode = ApiValidationMode.Strict)
            : base(initialize, configure, build, map)
        {
            _validationMode = validationMode;
        }

        private void Validate(WebApplication app)
        {
            if (_validationMode == ApiValidationMode.None)
                return;

            var manifest = new TManifest();
            var existingEndpoints = new CompositeEndpointDataSource(((IEndpointRouteBuilder)app).DataSources).Endpoints;
            var definedEndpoints = manifest.AllEndpoints;
            var defonedScopes = manifest.AllScopes;

            var rightByKey = existingEndpoints.ToLookup(e =>
            {
                var pattern = (e as RouteEndpoint)?.RoutePattern.RawText ?? e.Metadata.GetMetadata<IRouteDiagnosticsMetadata>()?.Route ?? "";
                var methods = e.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods ?? [];
                return $"{string.Join(",", methods.OrderBy(m => m))} {pattern.TrimEnd('/')}";
            });

            var matchedRightKeys = new HashSet<string>();
            foreach (var definedEndpoint in definedEndpoints)
            {
                var key = $"{definedEndpoint.Method.Method} {definedEndpoint.Path.TrimEnd('/')}";
                var matches = rightByKey[key];
                if(matches.Any())
                {
                    foreach (var existingEndpoint in matches)
                    {
                        matchedRightKeys.Add(key);

                        // Matched endpoint

                        var existingQueryParameters = existingEndpoint.Metadata
                            .OfType<IParameterBindingMetadata>()
                            .Select(p => p.ParameterInfo.GetCustomAttribute<FromQueryAttribute>() is FromQueryAttribute attr ? attr?.Name ?? p.ParameterInfo.Name : null)
                            .Where(n => n is not null);
                        if(!definedEndpoint.QueryParameters.SequenceEqual(existingQueryParameters))
                        {
                            var message = $"The query parameters of the endpoints '{key}' don't match (definded: {string.Join(",", definedEndpoint.QueryParameters)}, existing: {string.Join(",", existingQueryParameters)})";
                            if (_validationMode == ApiValidationMode.Strict)
                                throw new InvalidProgramException(message);
                            else
                                app.Logger.LogWarning(message);
                        }

                        var existingMetadata = existingEndpoint.Metadata.GetOrderedMetadata<IApiEndpointMetadata>();
                        if(!definedEndpoint.Metadata.SequenceEqual(existingMetadata))
                        {
                            var message = $"The metadata of the endpoints '{key}' don't match (definded: {string.Join(",", definedEndpoint.Metadata)}, existing: {string.Join(",", existingMetadata)})";
                            if (_validationMode == ApiValidationMode.Strict)
                                throw new InvalidProgramException(message);
                            else
                                app.Logger.LogWarning(message);
                        }
                    }
                }
                else
                {
                    // Not existing

                    var message = $"The defined endpoint '{key}' does not exist";
                    if (_validationMode == ApiValidationMode.Strict)
                        throw new InvalidProgramException(message);
                    else
                        app.Logger.LogWarning(message);
                }
            }

            foreach (var group in rightByKey)
            {
                if(!matchedRightKeys.Contains(group.Key))
                    foreach (var existingEndpoint in group)
                    {
                        // Not defined

                        var message = $"The existing endpoint '{group.Key}' is not defined";
                        if (_validationMode == ApiValidationMode.Strict)
                            throw new InvalidProgramException(message);
                        else
                            app.Logger.LogWarning(message);
                    }
            }
        }

        protected override async Task<WebApplication> BuildAsync(string[] args)
        {
            var app = await base.BuildAsync(args);
            Validate(app);
            return app;
        }
    }
}