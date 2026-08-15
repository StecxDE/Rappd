namespace Rappd.CQRS;

/// <summary>
/// Represents the handler of a request.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResponseBase
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Represents the handler of a request.
/// </summary>
/// <typeparam name="TRequest">The type of the request itself.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public abstract record Handler<TRequest, TResponse> : IHandler<Request<TRequest, TResponse>, TResponse>
    where TRequest : Request<TRequest, TResponse>
    where TResponse : ResponseBase
{
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public abstract Task<TResponse> Handle(CancellationToken cancellationToken);
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public Task<TResponse> Handle(Request<TRequest, TResponse> request, CancellationToken cancellationToken)
    {
        return Handle(cancellationToken);
    }
}
/// <summary>
/// Represents the handler of a command.
/// </summary>
/// <typeparam name="TRequest">The type of the command itself.</typeparam>
public abstract record CommandHandler<TRequest> : Handler<TRequest, Response>
    where TRequest : Command<TRequest>
{
    /// <summary>
    /// Utility to access common results.
    /// </summary>
    protected static CommonResults Results { get; } = CommonResults.Results;
    /// <summary>
    /// The method handling the request.
    /// </summary>
    /// <param name="cancellationToken">The provided cancellation token.</param>
    /// <returns>The result of the handler.</returns>
    public abstract Task<Result> HandleAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public override async Task<Response> Handle(CancellationToken cancellationToken)
    {
        Result result;
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                result = Results.Cancelled;
            }
            else
            {
                result = await HandleAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            result = Results.Cancelled;
        }
        catch (Exception ex)
        {
            result = Results.Exception(ex);
        }
        return new Response(result);
    }
}
/// <summary>
/// Represents the handler of a command with return data.
/// </summary>
/// <typeparam name="TRequest">The type of the command itself.</typeparam>
/// <typeparam name="TData">The type of the data returned by the command.</typeparam>
public abstract record CommandHandler<TRequest, TData> : Handler<TRequest, Response<TData>>
    where TRequest : Command<TRequest, TData>
{
    /// <summary>
    /// Utility to access common results.
    /// </summary>
    protected static CommonResults Results { get; } = CommonResults.Results;
    /// <summary>
    /// The method handling the request.
    /// </summary>
    /// <param name="cancellationToken">The provided cancellation token.</param>
    /// <returns>The result of the handler.</returns>
    public abstract Task<Result<TData>> HandleAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public override async Task<Response<TData>> Handle(CancellationToken cancellationToken)
    {
        Result<TData> result;
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                result = Results.Cancelled;
            }
            else
            {
                result = await HandleAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            result = Results.Cancelled;
        }
        catch (Exception ex)
        {
            result = Results.Exception(ex);
        }
        return new Response<TData>(result);
    }
}
/// <summary>
/// Represents the handler of a query without return data.
/// </summary>
/// <typeparam name="TRequest">The type of the command itself.</typeparam>
public abstract record QueryHandler<TRequest> : Handler<TRequest, Response>
    where TRequest : Query<TRequest>
{
    /// <summary>
    /// Utility to access common results.
    /// </summary>
    protected static CommonResults Results { get; } = CommonResults.Results;
    /// <summary>
    /// The method handling the request.
    /// </summary>
    /// <param name="cancellationToken">The provided cancellation token.</param>
    /// <returns>The result of the handler.</returns>
    public abstract Task<Result> HandleAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public override async Task<Response> Handle(CancellationToken cancellationToken)
    {
        Result result;
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                result = Results.Cancelled;
            }
            else
            {
                result = await HandleAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            result = Results.Cancelled;
        }
        catch (Exception ex)
        {
            result = Results.Exception(ex);
        }
        return new Response(result);
    }
}
/// <summary>
/// Represents the handler of a query.
/// </summary>
/// <typeparam name="TRequest">The type of the query itself.</typeparam>
/// <typeparam name="TData">The type of the data returned by the query.</typeparam>
public abstract record QueryHandler<TRequest, TData> : Handler<TRequest, Response<TData>>
    where TRequest : Query<TRequest, TData>
{
    /// <summary>
    /// Utility to access common results.
    /// </summary>
    protected static CommonResults Results { get; } = CommonResults.Results;
    /// <summary>
    /// The method handling the request.
    /// </summary>
    /// <param name="cancellationToken">The provided cancellation token.</param>
    /// <returns>The result of the handler.</returns>
    public abstract Task<Result<TData>> HandleAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public override async Task<Response<TData>> Handle(CancellationToken cancellationToken)
    {
        Result<TData> result;
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                result = Results.Cancelled;
            }
            else
            {
                result = await HandleAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            result = Results.Cancelled;
        }
        catch (Exception ex)
        {
            result = Results.Exception(ex);
        }
        return new Response<TData>(result);
    }
}

/// <summary>
/// Represents the handler of a request with arguments.
/// </summary>
/// <typeparam name="TRequest">The type of the request itself.</typeparam>
/// <typeparam name="TArguments">The type of the arguments.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public abstract record Handler<TRequest, TArguments, TResponse> : IHandler<Request<TRequest, TArguments, TResponse>, TResponse>
    where TRequest : Request<TRequest, TArguments, TResponse>
    where TResponse : ResponseBase
{
    /// <summary>
    /// Utility to access common results.
    /// </summary>
    protected static CommonResults Results { get; } = CommonResults.Results;
    /// <summary>
    /// The arguments provided with the request.
    /// </summary>
    protected TArguments Arguments { get; private set; } = default!;

    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public abstract Task<TResponse> Handle(CancellationToken cancellationToken);
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public Task<TResponse> Handle(Request<TRequest, TArguments, TResponse> request, CancellationToken cancellationToken)
    {
        Arguments = request.Arguments;
        return Handle(cancellationToken);
    }
}
/// <summary>
/// Represents the handler of a command with arguments.
/// </summary>
/// <typeparam name="TRequest">The type of the command itself.</typeparam>
/// <typeparam name="TArguments">The type of the arguments.</typeparam>
public abstract record ParameterizedCommandHandler<TRequest, TArguments> : Handler<TRequest, TArguments, Response>
    where TRequest : ParameterizedCommand<TRequest, TArguments>
{
    /// <summary>
    /// The method handling the request.
    /// </summary>
    /// <param name="cancellationToken">The provided cancellation token.</param>
    /// <returns>The result of the handler.</returns>
    public abstract Task<Result> HandleAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public override async Task<Response> Handle(CancellationToken cancellationToken)
    {
        Result result;
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                result = Results.Cancelled;
            }
            else
            {
                result = await HandleAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            result = Results.Cancelled;
        }
        catch (Exception ex)
        {
            result = Results.Exception(ex);
        }
        return new Response(result);
    }
}
/// <summary>
/// Represents the handler of a command with arguments and return data.
/// </summary>
/// <typeparam name="TRequest">The type of the command itself.</typeparam>
/// <typeparam name="TArguments">The type of the arguments.</typeparam>
/// <typeparam name="TData">The type of the data returned by the command.</typeparam>
public abstract record ParameterizedCommandHandler<TRequest, TArguments, TData> : Handler<TRequest, TArguments, Response<TData>>
    where TRequest : ParameterizedCommand<TRequest, TArguments, TData>
{
    /// <summary>
    /// The method handling the request.
    /// </summary>
    /// <param name="cancellationToken">The provided cancellation token.</param>
    /// <returns>The result of the handler.</returns>
    public abstract Task<Result<TData>> HandleAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public override async Task<Response<TData>> Handle(CancellationToken cancellationToken)
    {
        Result<TData> result;
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                result = Results.Cancelled;
            }
            else
            {
                result = await HandleAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            result = Results.Cancelled;
        }
        catch (Exception ex)
        {
            result = Results.Exception(ex);
        }
        return new Response<TData>(result);
    }
}
/// <summary>
/// Represents the handler of a query with arguments and without return data.
/// </summary>
/// <typeparam name="TRequest">The type of the query itself.</typeparam>
/// <typeparam name="TArguments">The type of the arguments.</typeparam>
public abstract record ParameterizedQueryHandler<TRequest, TArguments> : Handler<TRequest, TArguments, Response>
    where TRequest : ParameterizedQuery<TRequest, TArguments>
{
    /// <summary>
    /// The method handling the request.
    /// </summary>
    /// <param name="cancellationToken">The provided cancellation token.</param>
    /// <returns>The result of the handler.</returns>
    public abstract Task<Result> HandleAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public override async Task<Response> Handle(CancellationToken cancellationToken)
    {
        Result result;
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                result = Results.Cancelled;
            }
            else
            {
                result = await HandleAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            result = Results.Cancelled;
        }
        catch (Exception ex)
        {
            result = Results.Exception(ex);
        }
        return new Response(result);
    }
}
/// <summary>
/// Represents the handler of a query with arguments.
/// </summary>
/// <typeparam name="TRequest">The type of the query itself.</typeparam>
/// <typeparam name="TArguments">The type of the arguments.</typeparam>
/// <typeparam name="TData">The type of the data returned by the query.</typeparam>
public abstract record ParameterizedQueryHandler<TRequest, TArguments, TData> : Handler<TRequest, TArguments, Response<TData>>
    where TRequest : ParameterizedQuery<TRequest, TArguments, TData>
{
    /// <summary>
    /// The method handling the request.
    /// </summary>
    /// <param name="cancellationToken">The provided cancellation token.</param>
    /// <returns>The result of the handler.</returns>
    public abstract Task<Result<TData>> HandleAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public override async Task<Response<TData>> Handle(CancellationToken cancellationToken)
    {
        Result<TData> result;
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                result = Results.Cancelled;
            }
            else
            {
                result = await HandleAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            result = Results.Cancelled;
        }
        catch (Exception ex)
        {
            result = Results.Exception(ex);
        }
        return new Response<TData>(result);
    }
}