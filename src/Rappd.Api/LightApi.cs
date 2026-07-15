using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        /// <param name="configuration">The builder used to initialize the configuration of the <see cref="LightApi"/>.</param>
        /// <param name="logging">The builder used to initialize the logging of the <see cref="LightApi"/>.</param>
        public delegate void InitializationHandler(IConfigurationBuilder configuration, ILoggingBuilder logging);
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
        /// <summary>
        /// Runs the <see cref="LightApi"/> asyncronously.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>A <see cref="Task"/> that represents the entire runtime of the <see cref="LightApi"/> from startup to shutdown.</returns>
        public async Task RunAsync(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Initialize if possible
            _initialize?.Invoke(builder.Configuration, builder.Logging);
            // Configure if possible
            _configure?.Invoke(builder.Configuration, builder.Services);
            var app = builder.Build();
            // Build if possible
            _build?.Invoke((app.Services, app.Configuration, app.Environment, app.Lifetime, app.Logger, app.Urls), app);
            // Map if possible
            _map?.Invoke(app.Configuration, app);
            await app.RunAsync();
        }
    }
}