namespace Rappd.CQRS;

/// <summary>
/// Represents the response of a request.
/// </summary>
public abstract class Response
{
    /// <summary>
    /// Indicates if the request was successful.
    /// </summary>
    public abstract bool IsSuccess { get; }
    /// <summary>
    /// The occurred error if the request was not successful.
    /// </summary>
    public abstract ErrorResult? Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Response"/> class.
    /// </summary>
    private protected Response() { }

    /// <summary>
    /// Throws an exception if the request was not successful.
    /// </summary>
    /// <exception cref="Exception">Thrown if the request was not successful.</exception>
    public void EnsureSuccess() { if (!IsSuccess) { throw Error?.ToException() ?? new UnknownErrorResult().ToException(); } }
}

/// <summary>
/// The response of a command.
/// </summary>
public sealed class CommandResponse : Response
{
    /// <summary>
    /// Indicates if the command was successful.
    /// </summary>
    public override bool IsSuccess { get; }
    /// <summary>
    /// The occurred error if the command was not successful.
    /// </summary>
    public override ErrorResult? Error { get; }
    /// <summary>
    /// The returned result if the command was successful.
    /// </summary>
    public Result? Result { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref=CommandResponse"/> class with a result.
    /// </summary>
    /// <param name="result">The result of the command.</param>
    internal CommandResponse(Result result)
        => (IsSuccess, Error, Result) = result is ErrorResult error ? (false, error, (Result?)null) : (true, null, result);
}

/// <summary>
/// The response of a query.
/// </summary>
/// <typeparam name="TData">The type of the data returned by the query.</typeparam>
public sealed class QueryResponse<TData> : Response
{
    /// <summary>
    /// Indicates if the query was successful.
    /// </summary>
    public override bool IsSuccess { get; }
    /// <summary>
    /// The occurred error if the query was not successful.
    /// </summary>
    public override ErrorResult? Error { get; }
    /// <summary>
    /// The returned data if the query was successful.
    /// </summary>
    public TData Result { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref=QueryResponse{TData}"/> class with a result.
    /// </summary>
    /// <param name="result">The result of the query.</param>
    internal QueryResponse(Result<TData> result)
        => (IsSuccess, Error, Result) = result is ErrorProxy<TData> proxy ? (false, proxy.Error, result.Data) : (true, null, result.Data);

    /// <summary>
    /// Implicitly converts the given query response to the returned data.
    /// </summary>
    /// <param name="response">The query response to convert.</param>
    /// <exception cref="Exception">Thrown if the request was not successful.</exception>
    public static implicit operator TData(QueryResponse<TData> response)
        => response.IsSuccess ? response.Result : throw response.Error!.ToException();
}