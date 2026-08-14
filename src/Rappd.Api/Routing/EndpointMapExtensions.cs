using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rappd.Api
{
    /// <summary>
    /// Extension methods to simplify route mapping.
    /// </summary>
    public static class EndpointMapExtensions
    {
        /// <summary>
        /// Adds the given <see cref="IEndpointMap"/>s to the <see cref="IEndpointRouteBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the maps to.</param>
        /// <param name="endpoints">The endpoints to add.</param>
        /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointRouteBuilder Map(this IEndpointRouteBuilder builder, params IEndpointMap[] endpoints)
            => Map(builder, null, endpoints);
        /// <summary>
        /// Adds the given <see cref="IEndpointMap"/>s to the <see cref="IEndpointRouteBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to add the maps to.</param>
        /// <param name="path">The prefix prepended to all maps.</param>
        /// <param name="endpoints">The endpoints to add.</param>
        /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointRouteBuilder Map(this IEndpointRouteBuilder builder, string? path, params IEndpointMap[] endpoints)
        {
            foreach (var endpoint in endpoints)
                endpoint.AddTo(builder, path, [], []);
            return builder;
        }

        /// <summary>
        /// Creates a <see cref="GroupedEndpointMap"/> that matches requests with the specified prefix.
        /// </summary>
        /// <param name="path">The prefix which gets added for all sub maps.</param>
        /// <param name="endpoints">The sub maps.</param>
        /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
        private static GroupedEndpointMap CreateMap(string path, IEndpointMap[] endpoints)
            => new(path, endpoints);
        /// <summary>
        /// Creates a <see cref="DelegateEndpointMap"/> that matches requests with the specified method for the specified path.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="method">The request method.</param>
        /// <param name="delegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
        private static DelegateEndpointMap CreateMap(string path, string method, Delegate @delegate)
            => new(path, method, @delegate);


        /// <summary>
        /// Creates a <see cref="IEndpointMap"/> that matches HTTP GET requests for the specified path.
        /// </summary>
        /// <param name="path">The prefix which gets added for all sub maps.</param>
        /// <param name="endpoints">The sub endpoints.</param>
        /// <returns>A <see cref="IEndpointMap"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointMap Map(this string path, params IEndpointMap[] endpoints)
            => CreateMap(path, endpoints);
        /// <summary>
        /// Creates a <see cref="IEndpointMap"/> that matches HTTP GET requests for the specified path.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="delegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="IEndpointMap"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointMap Get(this string path, Delegate @delegate)
            => CreateMap(path, HttpMethods.Get, @delegate);
        /// <summary>
        /// Creates a <see cref="IEndpointMap"/> that matches HTTP POST requests for the specified path.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="delegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="IEndpointMap"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointMap Post(this string path, Delegate @delegate)
            => CreateMap(path, HttpMethods.Post, @delegate);
        /// <summary>
        /// Creates a <see cref="IEndpointMap"/> that matches HTTP PATCH requests for the specified path.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="delegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="IEndpointMap"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointMap Patch(this string path, Delegate @delegate)
            => CreateMap(path, HttpMethods.Patch, @delegate);
        /// <summary>
        /// Creates a <see cref="IEndpointMap"/> that matches HTTP PUT requests for the specified path.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="delegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="IEndpointMap"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointMap Put(this string path, Delegate @delegate)
            => CreateMap(path, HttpMethods.Put, @delegate);
        /// <summary>
        /// Creates a <see cref="IEndpointMap"/> that matches HTTP DELETE requests for the specified path.
        /// </summary>
        /// <param name="path">The route path.</param>
        /// <param name="delegate">The delegate executed when the endpoint is matched.</param>
        /// <returns>A <see cref="IEndpointMap"/> that can be used to further customize the endpoint.</returns>
        public static IEndpointMap Delete(this string path, Delegate @delegate)
            => CreateMap(path, HttpMethods.Delete, @delegate);
    }
}
