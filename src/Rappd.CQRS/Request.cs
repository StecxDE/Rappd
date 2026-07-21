namespace Rappd.CQRS;

/// <summary>
/// Represents a request.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IRequest<TResponse>
    where TResponse : ResponseBase
{

}

/// <summary>
/// Represents a request.
/// </summary>
/// <typeparam name="TRequest">The type of the request itself.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public record Request<TRequest, TResponse> : IRequest<TResponse> 
    where TRequest : IRequest<TResponse>
    where TResponse : ResponseBase
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
public abstract record Command<TRequest>() : Request<TRequest, Response>
    where TRequest : Command<TRequest>, IRequest<Response>
{
    /// <summary>
    /// The base type to easily implement a handler for the command.
    /// </summary>
    public abstract record Handler : CommandHandler<TRequest>;

    /// <summary>
    /// Sends the command.
    /// </summary>
    /// <returns>The response of the command.</returns>
    public static Task<Response> SendAsync()
        => SendAsync(default);
    /// <summary>
    /// Sends the command.
    /// </summary>
    /// <returns>The response of the command.</returns>
    public static async Task<Response> SendAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await SendRequestAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (NoHandlerFoundException)
        {
            return new Response(new NoHandlerResult());
        }
    }
}
/// <summary>
/// Represents a command with return data.
/// </summary>
/// <typeparam name="TRequest">The type of the command itself.</typeparam>
/// <typeparam name="TData">The type of the data returned by the command.</typeparam>
public abstract record Command<TRequest, TData>() : Request<TRequest, Response<TData>>
    where TRequest : Command<TRequest, TData>, IRequest<Response<TData>>
{
    /// <summary>
    /// The base type to easily implement a handler for the command.
    /// </summary>
    public abstract record Handler : CommandHandler<TRequest, TData>;

    /// <summary>
    /// Sends the command.
    /// </summary>
    /// <returns>The response of the command.</returns>
    public static Task<Response<TData>> SendAsync()
        => SendAsync(default);
    /// <summary>
    /// Sends the command.
    /// </summary>
    /// <returns>The response of the command.</returns>
    public static async Task<Response<TData>> SendAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await SendRequestAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (NoHandlerFoundException)
        {
            return new Response<TData>(new NoHandlerResult());
        }
    }
}
/// <summary>
/// Represents a query without return data.
/// </summary>
/// <typeparam name="TRequest">The type of the query itself.</typeparam>
public abstract record Query<TRequest>() : Request<TRequest, Response>
    where TRequest : Query<TRequest>, IRequest<Response>
{
    /// <summary>
    /// The base type to easily implement a handler for the query.
    /// </summary>
    public abstract record Handler : QueryHandler<TRequest>;

    /// <summary>
    /// Sends the query.
    /// </summary>
    /// <returns>The response of the query.</returns>
    public static Task<Response> SendAsync()
        => SendAsync(default);
    /// <summary>
    /// Sends the query.
    /// </summary>
    /// <returns>The response of the query.</returns>
    public static async Task<Response> SendAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await SendRequestAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (NoHandlerFoundException)
        {
            return new Response(new NoHandlerResult());
        }
    }
}
/// <summary>
/// Represents a query.
/// </summary>
/// <typeparam name="TRequest">The type of the query itself.</typeparam>
/// <typeparam name="TData">The type of the data returned by the query.</typeparam>
public abstract record Query<TRequest, TData>() : Request<TRequest, Response<TData>>
    where TRequest : Query<TRequest, TData>, IRequest<Response<TData>>
{
    /// <summary>
    /// The base type to easily implement a handler for the query.
    /// </summary>
    public abstract record Handler : QueryHandler<TRequest, TData>;

    /// <summary>
    /// Sends the query.
    /// </summary>
    /// <returns>The response of the query.</returns>
    public static Task<Response<TData>> SendAsync()
        => SendAsync(default);
    /// <summary>
    /// Sends the query.
    /// </summary>
    /// <returns>The response of the query.</returns>
    public static async Task<Response<TData>> SendAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await SendRequestAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (NoHandlerFoundException)
        {
            return new Response<TData>(new NoHandlerResult());
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
    where TResponse : ResponseBase
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
public abstract record ParameterizedCommand<TRequest, TArguments>() : Request<TRequest, TArguments, Response>(default(TArguments)!)
    where TRequest : ParameterizedCommand<TRequest, TArguments>, IRequest<Response>
{
    /// <summary>
    /// The base type to easily implement a handler for the command.
    /// </summary>
    public abstract record Handler : ParameterizedCommandHandler<TRequest, TArguments>;

    /// <summary>
    /// Sends the command.
    /// </summary>
    /// <param name="arguments">The arguments to be passed to the command.</param>
    /// <returns>The response of the command.</returns>
    public static Task<Response> SendAsync(TArguments arguments)
        => SendAsync(arguments, default);
    /// <summary>
    /// Sends the command.
    /// </summary>
    /// <param name="arguments">The arguments to be passed to the command.</param>
    /// <returns>The response of the command.</returns>
    public static async Task<Response> SendAsync(TArguments arguments, CancellationToken cancellationToken)
    {
        try
        {
            return await SendRequestAsync(arguments, cancellationToken).ConfigureAwait(false);
        }
        catch (NoHandlerFoundException)
        {
            return new Response(new NoHandlerResult());
        }
    }
}
/// <summary>
/// Represents a command with arguments and return data.
/// </summary>
/// <typeparam name="TRequest">The type of the command itself.</typeparam>
/// <typeparam name="TArguments">The type of the arguments.</typeparam>
/// <typeparam name="TData">The type of the data returned by the command.</typeparam>
public abstract record ParameterizedCommand<TRequest, TArguments, TData>() : Request<TRequest, TArguments, Response<TData>>(default(TArguments)!)
    where TRequest : ParameterizedCommand<TRequest, TArguments, TData>, IRequest<Response<TData>>
{
    /// <summary>
    /// The base type to easily implement a handler for the command.
    /// </summary>
    public abstract record Handler : ParameterizedCommandHandler<TRequest, TArguments, TData>;

    /// <summary>
    /// Sends the command.
    /// </summary>
    /// <param name="arguments">The arguments to be passed to the command.</param>
    /// <returns>The response of the command.</returns>
    public static Task<Response<TData>> SendAsync(TArguments arguments)
        => SendAsync(arguments, default);
    /// <summary>
    /// Sends the command.
    /// </summary>
    /// <param name="arguments">The arguments to be passed to the command.</param>
    /// <returns>The response of the command.</returns>
    public static async Task<Response<TData>> SendAsync(TArguments arguments, CancellationToken cancellationToken)
    {
        try
        {
            return await SendRequestAsync(arguments, cancellationToken).ConfigureAwait(false);
        }
        catch (NoHandlerFoundException)
        {
            return new Response<TData>(new NoHandlerResult());
        }
    }
}
/// <summary>
/// Represents a query with arguments and without return data.
/// </summary>
/// <typeparam name="TRequest">The type of the query itself.</typeparam>
/// <typeparam name="TArguments">The type of the arguments.</typeparam>
public abstract record ParameterizedQuery<TRequest, TArguments>() : Request<TRequest, TArguments, Response>(default(TArguments)!)
    where TRequest : ParameterizedQuery<TRequest, TArguments>, IRequest<Response>
{
    /// <summary>
    /// The base type to easily implement a handler for the query.
    /// </summary>
    public abstract record Handler : ParameterizedQueryHandler<TRequest, TArguments>;

    /// <summary>
    /// Sends the query.
    /// </summary>
    /// <param name="arguments">The arguments to be passed to the query.</param>
    /// <returns>The response of the query.</returns>
    public static Task<Response> SendAsync(TArguments arguments)
        => SendAsync(arguments, default);
    /// <summary>
    /// Sends the query.
    /// </summary>
    /// <param name="arguments">The arguments to be passed to the query.</param>
    /// <returns>The response of the query.</returns>
    public static async Task<Response> SendAsync(TArguments arguments, CancellationToken cancellationToken)
    {
        try
        {
            return await SendRequestAsync(arguments, cancellationToken).ConfigureAwait(false);
        }
        catch (NoHandlerFoundException)
        {
            return new Response(new NoHandlerResult());
        }
    }
}
/// <summary>
/// Represents a query with arguments.
/// </summary>
/// <typeparam name="TRequest">The type of the query itself.</typeparam>
/// <typeparam name="TArguments">The type of the arguments.</typeparam>
/// <typeparam name="TData">The type of the data returned by the query.</typeparam>
public abstract record ParameterizedQuery<TRequest, TArguments, TData>() : Request<TRequest, TArguments, Response<TData>>(default(TArguments)!)
    where TRequest : ParameterizedQuery<TRequest, TArguments, TData>, IRequest<Response<TData>>
{
    /// <summary>
    /// The base type to easily implement a handler for the query.
    /// </summary>
    public abstract record Handler : ParameterizedQueryHandler<TRequest, TArguments, TData>;

    /// <summary>
    /// Sends the query.
    /// </summary>
    /// <param name="arguments">The arguments to be passed to the query.</param>
    /// <returns>The response of the query.</returns>
    public static Task<Response<TData>> SendAsync(TArguments arguments)
        => SendAsync(arguments, default);
    /// <summary>
    /// Sends the query.
    /// </summary>
    /// <param name="arguments">The arguments to be passed to the query.</param>
    /// <returns>The response of the query.</returns>
    public static async Task<Response<TData>> SendAsync(TArguments arguments, CancellationToken cancellationToken)
    {
        try
        {
            return await SendRequestAsync(arguments, cancellationToken).ConfigureAwait(false);
        }
        catch (NoHandlerFoundException)
        {
            return new Response<TData>(new NoHandlerResult());
        }
    }
}