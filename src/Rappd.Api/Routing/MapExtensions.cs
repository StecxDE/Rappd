using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rappd.Api
{
    /// <summary>
    /// Extension methods to simplify route mapping.
    /// </summary>
    public static class MapExtensions
    {
        /// <summary>
        /// Adds the given <see cref="Api.Map"/> to the <see cref="IEndpointRouteBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the maps to.</param>
        /// <param name="maps">The maps to add.</param>
        /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointRouteBuilder Map(this IEndpointRouteBuilder builder, params Map[] maps)
            => Map(builder, null, maps);
        /// <summary>
        /// Adds the given <see cref="Api.Map"/> to the <see cref="IEndpointRouteBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the maps to.</param>
        /// <param name="prefix">The prefix prepended to all maps.</param>
        /// <param name="maps">The maps to add.</param>
        /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointRouteBuilder Map(this IEndpointRouteBuilder builder, string? prefix, params Map[] maps)
        {
            foreach (var map in maps)
            {
                var route = map.AddTo(builder, prefix);
                foreach (var convention in map.Conventions)
                    route.Add(convention);
            }
            return builder;
        }

        /// <summary>
        /// Creates a pattern which can be used to map method with the <see cref="IEndpointConventionBuilder"/> out of a prefix and a path.
        /// </summary>
        /// <param name="prefix">The specified prefix.</param>
        /// <param name="path">The specified path.</param>
        /// <returns>A pattern which can be used to map method with the <see cref="IEndpointConventionBuilder"/>.</returns>
        private static string CreatePattern(string? prefix, string path)
        {
            // Make sure that the prefix has exacly one trailing slash
            if (prefix is not null)
                prefix = prefix.Trim('/') + "/";
            return prefix + path.TrimStart('/');
        }
        /// <summary>
        /// Creates a <see cref="Api.Map"/> that matches requests with the specified method for the specified path.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="method">The request method.</param>
        /// <param name="delegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="Api.Map"/> that can be added to the <see cref="IEndpointConventionBuilder"/>.</returns>
        private static Map Map(string path, string method, Delegate @delegate)
            => new(path, (m, b, p) => b.MapMethods(CreatePattern(p, m.Path), [method], m.Delegate), @delegate);

        /// <summary>
        /// Creates a <see cref="Api.Map"/> that matches HTTP GET requests for the specified path.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="delegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="Api.Map"/> that can be added to the <see cref="IEndpointConventionBuilder"/>.</returns>
        public static Map Get(this string path, Delegate @delegate)
            => Map(path, HttpMethods.Get, @delegate);
        /// <summary>
        /// Creates a <see cref="Api.Map"/> that matches HTTP POST requests for the specified path.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="delegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="Api.Map"/> that can be added to the <see cref="IEndpointConventionBuilder"/>.</returns>
        public static Map Post(this string path, Delegate @delegate)
            => Map(path, HttpMethods.Post, @delegate);
        /// <summary>
        /// Creates a <see cref="Api.Map"/> that matches HTTP PATCH requests for the specified path.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="delegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="Api.Map"/> that can be added to the <see cref="IEndpointConventionBuilder"/>.</returns>
        public static Map Patch(this string path, Delegate @delegate)
            => Map(path, HttpMethods.Patch, @delegate);
        /// <summary>
        /// Creates a <see cref="Api.Map"/> that matches HTTP PUT requests for the specified path.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="delegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="Api.Map"/> that can be added to the <see cref="IEndpointConventionBuilder"/>.</returns>
        public static Map Put(this string path, Delegate @delegate)
            => Map(path, HttpMethods.Put, @delegate);
        /// <summary>
        /// Creates a <see cref="Api.Map"/> that matches HTTP DELETE requests for the specified path.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="delegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="Api.Map"/> that can be added to the <see cref="IEndpointConventionBuilder"/>.</returns>
        public static Map Delete(this string path, Delegate @delegate)
            => Map(path, HttpMethods.Delete, @delegate);
    }
}