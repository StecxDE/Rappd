using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Rappd.Api
{
    /// <summary>
    /// The type of the method used to actually add the <see cref="Map"/> to the <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <param name="map">The <see cref="Map"/> to add.</param>
    /// <param name="route">The <see cref="IEndpointRouteBuilder"/>.</param>
    /// <param name="prefix">The prefix to prepend to the <see cref="Map"/> path.</param>
    /// <returns></returns>
    public delegate IEndpointConventionBuilder Mapper(Map map, IEndpointRouteBuilder route, string? prefix = null);
    /// <summary>
    /// Represents the mapping of a path to a delegate.
    /// </summary>
    public class Map : IEndpointConventionBuilder
    {
        /// <summary>
        /// The method used to actually add the <see cref="Map"/> to the <see cref="IEndpointRouteBuilder"/>.
        /// </summary>
        private readonly Mapper _mapper;

        /// <summary>
        /// The path to map.
        /// </summary>
        public string Path { get; }
        /// <summary>
        /// The delegate to map.
        /// </summary>
        public Delegate Delegate { get; }
        /// <summary>
        /// The conventions to apply.
        /// </summary>
        public List<Action<EndpointBuilder>> Conventions { get; } = new();

        /// <summary>
        /// Creates a new <see cref="Map"/> with the given path, mapper and delegate.
        /// </summary>
        /// <param name="path">The path to map.</param>
        /// <param name="mapper">The method used to actually add the <see cref="Map"/> to the <see cref="IEndpointRouteBuilder"/>.</param>
        /// <param name="delegate">The delegate to map.</param>
        internal Map(string path, Mapper mapper, Delegate @delegate)
            => (Path, _mapper, Delegate) = (path, mapper, @delegate);

        /// <summary>
        /// Adds the <see cref="Map"/> to the <see cref="IEndpointRouteBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/>.</param>
        /// <param name="prefix">The prefix to prepend to the <see cref="Map"/> path.</param>
        /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
        internal IEndpointConventionBuilder AddTo(IEndpointRouteBuilder builder, string? prefix = null)
            => _mapper(this, builder, prefix);
        /// <summary>
        /// Adds a convention to the <see cref="Map"/>.
        /// </summary>
        /// <param name="convention">The convention to add.</param>
        public void Add(Action<EndpointBuilder> convention)
            => Conventions.Add(convention);
    }
}