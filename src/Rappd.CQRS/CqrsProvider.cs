using System.Collections.Concurrent;
using System.Reflection;

namespace Rappd.CQRS;

#pragma warning disable CA1822

/// <summary>
/// The class resonsible to connect requests with its handlers.
/// </summary>
public static class CqrsProvider
{
    /// <summary>
    /// The handler registry used to register and provide the handlers.
    /// </summary>
    private static HandlerRegistry _registry = new(Array.Empty<Assembly>(), (t) => Activator.CreateInstance(t));

    /// <summary>
    /// Sends a request.
    /// </summary>
    /// <typeparam name="TResponse">The type of the returned response.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <exception cref="NoHandlerFoundException">Thrown if no handler was found for the request.</exception>
    /// <returns>The response of the request.</returns>
    internal static Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>
        where TResponse : Response
        => _registry.GetHandler<TRequest, TResponse>().Handle(request, cancellationToken);

    /// <summary>
    /// Configures the provider.
    /// </summary>
    /// <param name="searchAssemblies">The assemblies containing the possible handler types.</param>
    /// <param name="activator">The function used to create the handler instances.</param>
    public static void Configure(Assembly[] searchAssemblies, Func<Type, object?> activator)
        => _registry = new(searchAssemblies, activator);
    /// <summary>
    /// Registers a handler type manually.
    /// </summary>
    /// <typeparam name="THandler">The handler type to register.</typeparam>
    /// <exception cref="ArgumentException">Thrown if the given type is not a handler.</exception>
    public static void Register<THandler>()
        => _registry.RegisterHandlerType<THandler>();

    /// <summary>
    /// The handler registry used to register and provide the handlers.
    /// </summary>
    private class HandlerRegistry
    {
        /// <summary>
        /// The store for all already registered handler types.
        /// </summary>
        private readonly ConcurrentDictionary<Type, IEnumerable<Type>> _handlerTypes = new();
        /// <summary>
        /// The assemblies containing the possible handler types.
        /// </summary>
        private readonly Assembly[] _searchAssemblies;
        /// <summary>
        /// The function used to create the handler instances.
        /// </summary>
        private readonly Func<Type, object?> _activator;

        /// <summary>
        /// Initializes a new instance of the <see cref=HandlerRegistry"/> class with assemblies and an activator.
        /// </summary>
        /// <param name="searchAssemblies">The assemblies containing the possible handler types.</param>
        /// <param name="activator">The function used to create the handler instances.</param>
        public HandlerRegistry(Assembly[] searchAssemblies, Func<Type, object?> activator)
            => (_searchAssemblies, _activator) = (searchAssemblies, activator);

        /// <summary>
        /// Checks the given type for exact or open generic equality to the comparer.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="comparer">The type compared with the given type.</param>
        /// <returns><see cref="true"/> if the type is equal to the comparer or if the types generic definition is equal to the comparer, otherwise <see cref="false"/>.</returns>
        private static bool CheckType(Type type, Type comparer)
            // If the comparer is a consrtuchted generic type check for exact match
            // If the type is a gneric type check for open generic eqality
            => ((!comparer.IsGenericType || comparer.IsConstructedGenericType) && comparer == type)
            || (type.IsGenericType && comparer == type.GetGenericTypeDefinition());

        /// <summary>
        /// Gets the request type of the given handler type if possible.
        /// </summary>
        /// <param name="handlerType">The handler type.</param>
        /// <returns>The request type of the given handler type if possible, otherwise <see cref="null"/>.</returns>
        private Type? GetRequestType(Type handlerType)
            // Try to get the first generic type argument of the first implemented IHandler<,> interface
            => handlerType.GetInterfaces().FirstOrDefault(i => CheckType(i, typeof(IHandler<,>)))?.GenericTypeArguments[0];
        /// <summary>
        /// Gets a handler type for the given request type if possible.
        /// </summary>
        /// <param name="requestType">The request type.</param>
        /// <returns>The handler type for the given request type if possible, otherwise <see cref="null"/>.</returns>
        private Type? GetHandlerType(Type requestType)
        {
            // Check if we didn't already register handlers for that request
            if (!_handlerTypes.ContainsKey(requestType))
            {
                // Use the given assemblies to search for handler types if existing,
                // otherwise use the assembly of the request type and the entry assembly
                var searchTagets = _searchAssemblies.Length > 0 ? _searchAssemblies : new[]
                {
                    requestType.GenericTypeArguments.FirstOrDefault()?.Assembly,
                    Assembly.GetEntryAssembly()
                }.Distinct();

                // Register all handler types for the given request type contained in all search assemblies
                _handlerTypes.TryAdd(requestType, searchTagets.SelectMany(a =>
                    a?.GetTypes().Where(t => GetRequestType(t) == requestType) ?? Array.Empty<Type>()
                ));
            }

            // Try to get the first reqistered handler with as much matching selection attributes as possible
            return _handlerTypes[requestType]
                .Select(handler => (handler, handler.GetCustomAttributes<HandlerSelectionAttribute>()))
                .OrderByDescending(i => i.Item2.Count())
                .FirstOrDefault(i => i.Item2.All(a => a.IsMatch()))
                .handler;
        }

        /// <summary>
        /// Registers a handler type manually.
        /// </summary>
        /// <typeparam name="THandler">The handler type to register.</param>
        /// <exception cref="ArgumentException">Thrown if the given type is not a handler.</exception>
        public void RegisterHandlerType<THandler>()
        {
            // Get the handler type
            var handlerType = typeof(THandler);
            // Get a request type for given handler type if possible
            var requestType = GetRequestType(handlerType) ?? throw new ArgumentException("The type is not a handler.", nameof(THandler));
            // Add the type as a handler for the request
            _handlerTypes.AddOrUpdate(requestType, (_) => new[] { handlerType }, (_, e) => e.Append(handlerType));
        }
        /// <summary>
        /// Gets a handler for the given request type
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response of the request.</typeparam>
        /// <returns>A handler for the request type.<returns>
        /// <exception cref="NoHandlerFoundException">Thrown if no handler was found.</exception>
        public IHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>()
            where TRequest : IRequest<TResponse>
            where TResponse : Response
        {
            // Get the request type
            var requestType = typeof(TRequest);
            // Get a handler type for given request type if possible
            var handlerType = GetHandlerType(requestType) ?? throw new NoHandlerFoundException();
            // Try to activate the handler type
            try
            {
                return _activator(handlerType) as IHandler<TRequest, TResponse> ?? throw new HandlerActivationException(handlerType);
            }
            catch (Exception ex)
            {
                throw new HandlerActivationException(handlerType, ex);
            }
        }
    }
}