using Rappd.Data;

namespace Rappd.CQRS;

/// <summary>
/// Represents the response of a request.
/// </summary>
public abstract class ResponseBase
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
    /// Initializes a new instance of the <see cref="ResponseBase"/> class.
    /// </summary>
    private protected ResponseBase() { }

    /// <summary>
    /// Throws an exception if the request was not successful.
    /// </summary>
    /// <exception cref="Exception">Thrown if the request was not successful.</exception>
    public void EnsureSuccess() { if (!IsSuccess) { throw Error?.ToException() ?? new UnknownErrorResult().ToException(); } }
}

/// <summary>
/// The response of a request.
/// </summary>
public sealed class Response : ResponseBase
{
    /// <summary>
    /// Indicates if the request was successful.
    /// </summary>
    public override bool IsSuccess { get; }
    /// <summary>
    /// The occurred error if the request was not successful.
    /// </summary>
    public override ErrorResult? Error { get; }
    /// <summary>
    /// The returned result if the request was successful.
    /// </summary>
    public Result? Result { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref=Response"/> class with a result.
    /// </summary>
    /// <param name="result">The result of the request.</param>
    internal Response(Result result)
        => (IsSuccess, Error, Result) = result is ErrorResult error ? (false, error, (Result?)null) : (true, null, result);
}

/// <summary>
/// The response of a request containing data.
/// </summary>
/// <typeparam name="TData">The type of the data returned by the request.</typeparam>
public sealed class Response<TData> : ResponseBase
{
    /// <summary>
    /// Indicates if the request was successful.
    /// </summary>
    public override bool IsSuccess { get; }
    /// <summary>
    /// The occurred error if the request was not successful.
    /// </summary>
    public override ErrorResult? Error { get; }
    /// <summary>
    /// The returned data if the request was successful.
    /// </summary>
    public TData Result { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref=Response{TData}"/> class with a result.
    /// </summary>
    /// <param name="result">The result of the request.</param>
    internal Response(Result<TData> result)
    {
        if (result is ErrorProxy<TData> proxy)
            (IsSuccess, Error, Result) = (false, proxy.Error, result.Data);
        else
        {
            var data = result.Data;
            if (TypeToInterfaceConverter.TryConvertTo<TData>(data, out var converted))
                data = converted;
            (IsSuccess, Error, Result) = (true, null, data);
        }
    }

    /// <summary>
    /// Implicitly converts the given response to the returned data.
    /// </summary>
    /// <param name="response">The response to convert.</param>
    /// <exception cref="Exception">Thrown if the request was not successful.</exception>
    public static implicit operator TData(Response<TData> response)
        => response.IsSuccess ? response.Result : throw response.Error!.ToException();
}