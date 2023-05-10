namespace Rappd.CQRS;

/// <summary>
/// Represents a request.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IRequest<TResponse>
    where TResponse : Response
{

}

/// <summary>
/// Represents a request.
/// </summary>
/// <typeparam name="TRequest">The type of the request itself.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public record Request<TRequest, TResponse> : IRequest<TResponse> 
    where TRequest : IRequest<TResponse>
    where TResponse : Response
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Request{TRequest, TResponse}"> class.
    /// </summary>
    internal Request() { }

    /// <summary>
    /// Sends the request.
    /// </summary>
    /// <exception cref="NoHandlerFoundException">Thrown if no handler was found for the request.</exception>
    /// <returns>The response of the request.</returns>
    protected static Task<TResponse> SendRequestAsync(CancellationToken cancellationToken)
        => CqrsProvider.SendAsync<Request<TRequest, TResponse>, TResponse>(new Request<TRequest, TResponse>(), cancellationToken);
}
/// <summary>
/// Represents a command.
/// </summary>
/// <typeparam name="TRequest">The type of the command itself.</typeparam>
public abstract record Command<TRequest>() : Request<TRequest, CommandResponse>
    where TRequest : Command<TRequest>, IRequest<CommandResponse>
{
    /// <summary>
    /// The base type to easily implement a handler for the command.
    /// </summary>
    public abstract record Handler : CommandHandler<TRequest>;

    /// <summary>
    /// Sends the command.
    /// </summary>
    /// <returns>The response of the command.</returns>
    public static async Task<CommandResponse> SendAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await SendRequestAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (NoHandlerFoundException)
        {
            return new CommandResponse(new NoHandlerResult());
        }
    }
}
/// <summary>
/// Represents a query.
/// </summary>
/// <typeparam name="TRequest">The type of the query itself.</typeparam>
/// <typeparam name="TData">The type of the data returned by the query.</typeparam>
public abstract record Query<TRequest, TData>() : Request<TRequest, QueryResponse<TData>>
    where TRequest : Query<TRequest, TData>, IRequest<QueryResponse<TData>>
{
    /// <summary>
    /// The base type to easily implement a handler for the query.
    /// </summary>
    public abstract record Handler : QueryHandler<TRequest, TData>;

    /// <summary>
    /// Sends the query.
    /// </summary>
    /// <returns>The response of the query.</returns>
    public static async Task<QueryResponse<TData>> SendAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await SendRequestAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (NoHandlerFoundException)
        {
            return new QueryResponse<TData>(new NoHandlerResult());
        }
    }
}

/// <summary>
/// Represents a request with arguments.
/// </summary>
/// <typeparam name="TRequest">The type of the request itself.</typeparam>
/// <typeparam name="TArguments">The type of the arguments.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public record Request<TRequest, TArguments, TResponse> : IRequest<TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Response
{
    /// <summary>
    /// The arguments passed to the request.
    /// </summary>
    public TArguments Arguments { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Request{TRequest, TResponse}"> class with the arguments passed to the request..
    /// </summary>
    /// <param name="arguments">The arguments passed to the request.</param>
    internal Request(TArguments arguments)
        => Arguments = arguments;

    /// <summary>
    /// Sends the request.
    /// </summary>
    /// <param name="arguments">The arguments to be passed to the request.</param>
    /// <exception cref="NoHandlerFoundException">Thrown if no handler was found for the request.</exception>
    /// <returns>The response of the request.</returns>
    protected static Task<TResponse> SendRequestAsync(TArguments arguments, CancellationToken cancellationToken)
        => CqrsProvider.SendAsync<Request<TRequest, TArguments, TResponse>, TResponse>(new Request<TRequest, TArguments, TResponse>(arguments), cancellationToken);
}
/// <summary>
/// Represents a command with arguments.
/// </summary>
/// <typeparam name="TRequest">The type of the command itself.</typeparam>
/// <typeparam name="TArguments">The type of the arguments.</typeparam>
public abstract record Command<TRequest, TArguments>() : Request<TRequest, TArguments, CommandResponse>(default(TArguments)!)
    where TRequest : Command<TRequest, TArguments>, IRequest<CommandResponse>
{
    /// <summary>
    /// The base type to easily implement a handler for the command.
    /// </summary>
    public abstract record Handler : CommandHandler<TRequest, TArguments>;

    /// <summary>
    /// Sends the command.
    /// </summary>
    /// <param name="arguments">The arguments to be passed to the command.</param>
    /// <returns>The response of the command.</returns>
    public static async Task<CommandResponse> SendAsync(TArguments arguments, CancellationToken cancellationToken = default)
    {
        try
        {
            return await SendRequestAsync(arguments, cancellationToken).ConfigureAwait(false);
        }
        catch (NoHandlerFoundException)
        {
            return new CommandResponse(new NoHandlerResult());
        }
    }
}
/// <summary>
/// Represents a query with arguments.
/// </summary>
/// <typeparam name="TRequest">The type of the query itself.</typeparam>
/// <typeparam name="TArguments">The type of the arguments.</typeparam>
/// <typeparam name="TData">The type of the data returned by the query.</typeparam>
public abstract record Query<TRequest, TArguments, TData>() : Request<TRequest, TArguments, QueryResponse<TData>>(default(TArguments)!)
    where TRequest : Query<TRequest, TArguments, TData>, IRequest<QueryResponse<TData>>
{
    /// <summary>
    /// The base type to easily implement a handler for the query.
    /// </summary>
    public abstract record Handler : QueryHandler<TRequest, TArguments, TData>;

    /// <summary>
    /// Sends the query.
    /// </summary>
    /// <param name="arguments">The arguments to be passed to the query.</param>
    /// <returns>The response of the query.</returns>
    public static async Task<QueryResponse<TData>> SendAsync(TArguments arguments, CancellationToken cancellationToken = default)
    {
        try
        {
            return await SendRequestAsync(arguments, cancellationToken).ConfigureAwait(false);
        }
        catch (NoHandlerFoundException)
        {
            return new QueryResponse<TData>(new NoHandlerResult());
        }
    }
}