namespace Rappd.CQRS;

/// <summary>
/// Represents the result of a request handler.
/// </summary>
public abstract class Result { }
/// <summary>
/// Represents an error occured in a request handler.
/// </summary>
public abstract class ErrorResult : Result
{
    /// <summary>
    /// A message describing the error.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResult"/> class with a message describing the error.
    /// </summary>
    /// <param name="message">A message describing the error.</param>
    public ErrorResult(string message)
        => Message = message;

    /// <summary>
    /// When overridden in a derived class, gets a exception describing the error.
    /// </summary>
    /// <returns>A exception describing the error.</returns>
    public abstract Exception ToException();
    /// <summary>
    /// Returns a string that represents the current error result.
    /// </summary>
    /// <returns>A string that represents the current error result.</returns>
    public override string ToString()
        => Message;
}
/// <summary>
/// Represents the result of a request handler containing returned data.
/// </summary>
/// <typeparam name="TData">The type of the data returned by the request handler.</typeparam>
public abstract class Result<TData> : Result
{
    /// <summary>
    /// The data returned by the request.
    /// </summary>
    public TData Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TData}"/> class with returned data.
    /// </summary>
    /// <param name="data">The data returned by the request.</param>
    public Result(TData data)
        => Data = data;

    /// <summary>
    /// Implicitly converts the given data to the default result of a successful request containing returned data.
    /// </summary>
    /// <param name="data">The data to convert.</param>
    public static implicit operator Result<TData>(TData data)
        => new OkResult<TData>(data);
    /// <summary>
    /// Implicitly converts the given error result to a error proxy.
    /// </summary>
    /// <param name="error">The error result to convert.</param>
    public static implicit operator Result<TData>(ErrorResult error)
        => new ErrorProxy<TData>(error);
}
/// <summary>
/// Wraps an error occured in a request handler in an result with data.
/// </summary>
/// <typeparam name="TData">The type of the data returned by the request handler.</typeparam>
internal sealed class ErrorProxy<TData> : Result<TData>
{
    /// <summary>
    /// The occurred error.
    /// </summary>
    internal ErrorResult Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorProxy{TData}"/> class with a error.
    /// </summary>
    /// <param name="error">The occurred error.</param>
    internal ErrorProxy(ErrorResult error) : base(default!)
        => Error = error;

    /// <summary>
    /// Implicitly converts the given error proxy to a error result.
    /// </summary>
    /// <param name="error">The error result to convert.</param>
    public static implicit operator ErrorResult(ErrorProxy<TData> error)
        => error.Error;
}

/// <summary>
/// The default result of a successful request.
/// </summary>
public sealed class OkResult : Result
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="OkResult"/> class.
    /// </summary>
    internal OkResult() { }
}
/// <summary>
/// The default result of a unsuccessful request when no exception occurred.
/// </summary>
public sealed class UnknownErrorResult : ErrorResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownErrorResult"/> class.
    /// </summary>
    internal UnknownErrorResult() : base("An unknown error occured.") { }

    /// <summary>
    /// Gets a exception describing the error.
    /// </summary>
    /// <returns>A exception describing the error.</returns>
    public override Exception ToException()
        => new(Message);
}
/// <summary>
/// The default result of a request without a handler.
/// </summary>
public sealed class NoHandlerResult : ErrorResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoHandlerResult"/> class.
    /// </summary>
    internal NoHandlerResult() : base("No handler found.") { }

    /// <summary>
    /// Gets a exception describing the error.
    /// </summary>
    /// <returns>A exception describing the error.</returns>
    public override Exception ToException()
        => new(Message);
}
/// <summary>
/// The default result of a cancelled request.
/// </summary>
public sealed class CancelledResult : ErrorResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CancelledResult"/> class.
    /// </summary>
    internal CancelledResult() : base("The request was cancelled.") { }

    /// <summary>
    /// Gets a exception describing the error.
    /// </summary>
    /// <returns>A exception describing the error.</returns>
    public override Exception ToException()
        => new(Message);
}
/// <summary>
/// The default result of a unsuccessful request when an exception occurred.
/// </summary>
public sealed class ExceptionResult : ErrorResult
{
    /// <summary>
    /// The occurred exception.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionResult"/> class with a occurred exception.
    /// </summary>
    /// <param name="exception">The occurred exception.</param>
    internal ExceptionResult(Exception exception) : base(exception.Message)
        => Exception = exception;

    /// <summary>
    /// Gets a exception describing the error.
    /// </summary>
    /// <returns>A exception describing the error.</returns>
    public override Exception ToException()
        => Exception;
    /// <summary>
    /// Returns a string that represents the current exception result.
    /// </summary>
    /// <returns>A string that represents the current exception result.</returns>
    public override string ToString()
        => Exception.ToString();
}
/// <summary>
/// The default result of a successful request containing returned data.
/// </summary>
/// <typeparam name="TData">The type of the returned data.</typeparam>
public sealed class OkResult<TData> : Result<TData>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OkResult{TData}"/> class with returned data.
    /// </summary>
    /// <param name="data">The data returned by the request.</param>
    internal OkResult(TData data) : base(data) { }
}

/// <summary>
/// Defines common results.
/// </summary>
public static class Results
{
    /// <summary>
    /// The default result of a successful request.
    /// </summary>
    public static Result Ok => new OkResult();
    /// <summary>
    /// The default result of a unsuccessful request when no exception occurred.
    /// </summary>
    public static ErrorResult Error => new UnknownErrorResult();
    /// <summary>
    /// The default result of a cancelled request.
    /// </summary>
    public static ErrorResult Cancelled => new CancelledResult();
    /// <summary>
    /// The default result of a successful request containing returned data.
    /// </summary>
    /// <typeparam name="TData">The type of the data to return.</typeparam>
    /// <param name="data">The data to be returned by the request.</param>
    /// <returns>A new instance of the <see cref="OkResult{TData}"/> class with returned data.</returns>
    public static Result<TData> Data<TData>(TData data) => new OkResult<TData>(data);
    /// <summary>
    /// The default result of a unsuccessful request when an exception occurred.
    /// </summary>
    /// <param name="exception">The occurred exception.</param>
    /// <returns>A new instance of the <see cref="ExceptionResult"/> class with a occurred exception.</returns>
    public static ErrorResult Exception(Exception exception) => new ExceptionResult(exception);
}