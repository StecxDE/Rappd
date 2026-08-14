using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Rappd.Api
{
    /// <summary>
    /// Represents a endpoint map
    /// </summary>
    public interface IEndpointMap : IEndpointConventionBuilder
    {
        /// <summary>
        /// Adds the <see cref="IEndpointMap"/> to the <see cref="IEndpointRouteBuilder"/>.
        /// </summary>
        /// <param name="prefix">The prefix added to the path to map.</param>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/>.</param>
        internal void AddTo(IEndpointRouteBuilder builder, string? prefix, Action<EndpointBuilder>[] conventions, Action<EndpointBuilder>[] finallyConventions);

        /// <summary>
        /// Creates a pattern which can be used to map method with the <see cref="IEndpointConventionBuilder"/> out of a prefix and the path.
        /// </summary>
        /// <param name="prefix">The specified prefix.</param>
        /// <returns>A pattern which can be used to map method with the <see cref="IEndpointConventionBuilder"/>.</returns>
        internal static string CreatePattern(string? prefix, string path)
            => (prefix + path).Replace("//", "/");
    }

    /// <summary>
    /// Represents the mapping of multiple maps with a prefix.
    /// </summary>
    /// <param name="path">The prefix which gets added for all sub maps.</param>
    /// <param name="maps">The sub maps.</param>
    internal sealed class GroupedEndpointMap(string path, IEndpointMap[] maps) : IEndpointMap
    {
        /// <summary>
        /// The conventions to apply.
        /// </summary>
        private readonly List<Action<EndpointBuilder>> _conventions = [];
        /// <summary>
        /// The conventions to apply finally.
        /// </summary>
        private readonly List<Action<EndpointBuilder>> _finallyConventions = [];

        /// <summary>
        /// Adds the <see cref="IEndpointMap"/> to the <see cref="IEndpointRouteBuilder"/>.
        /// </summary>
        /// <param name="prefix">The prefix added to the path to map.</param>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/>.</param>
        public void AddTo(IEndpointRouteBuilder builder, string? prefix, Action<EndpointBuilder>[] conventions, Action<EndpointBuilder>[] finallyConventions)
        {
            string pattern = IEndpointMap.CreatePattern(prefix, path);
            foreach (var map in maps)
                map.AddTo(builder, pattern, [..conventions, .._conventions], [..finallyConventions, .._finallyConventions]);
        }

        /// <summary>
        /// Adds a convention to the <see cref="DelegateEndpointMap"/>.
        /// </summary>
        /// <param name="convention">The convention to add.</param>
        public void Add(Action<EndpointBuilder> convention)
            => _conventions.Add(convention);

        /// <summary>
        /// Adds a finallly convention to the <see cref="DelegateEndpointMap"/>.
        /// </summary>
        /// <param name="convention">The convention to add.</param>
        public void Finally(Action<EndpointBuilder> finallyConvention)
            => _finallyConventions.Add(finallyConvention);
    }

    /// <summary>
    /// Represents the mapping of a path to a delegate.
    /// </summary>
    internal sealed class DelegateEndpointMap(
        string path, string method, Delegate @delegate
    ) : IEndpointMap
    {
        /// <summary>
        /// The conventions to apply.
        /// </summary>
        private readonly List<Action<EndpointBuilder>> _conventions = [];
        /// <summary>
        /// The conventions to apply finally.
        /// </summary>
        private readonly List<Action<EndpointBuilder>> _finallyConventions = [];

        public void AddTo(IEndpointRouteBuilder builder, string? prefix, Action<EndpointBuilder>[] conventions, Action<EndpointBuilder>[] finallyConventions)
        {
            var pattern = IEndpointMap.CreatePattern(prefix, path);
            var mappedEnpoint = builder.MapMethods(pattern, [method], @delegate);
            foreach (var convention in conventions)
                mappedEnpoint.Add(convention);
            foreach (var convention in _conventions)
                mappedEnpoint.Add(convention);
            foreach (var convention in finallyConventions)
                mappedEnpoint.Finally(convention);
            foreach (var convention in _finallyConventions)
                mappedEnpoint.Finally(convention);
        }
        /// <summary>
        /// Adds a convention to the <see cref="DelegateEndpointMap"/>.
        /// </summary>
        /// <param name="convention">The convention to add.</param>
        public void Add(Action<EndpointBuilder> convention)
            => _conventions.Add(convention);

        /// <summary>
        /// Adds a finallly convention to the <see cref="DelegateEndpointMap"/>.
        /// </summary>
        /// <param name="convention">The convention to add.</param>
        public void Finally(Action<EndpointBuilder> finallyConvention)
            => _finallyConventions.Add(finallyConvention);
    }
}
