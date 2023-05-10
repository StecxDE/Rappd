namespace Rappd.CQRS;

/// <summary>
/// The exception thrown when no handler was found.
/// </summary>
internal class NoHandlerFoundException : Exception
{

}

/// <summary>
/// The exception thrown when a handler can't be activated.
/// </summary>
public class HandlerActivationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref=HandlerActivationException"/> class with the handler type.
    /// </summary>
    /// <param name="handlerType">The handler type failed to activate.</param>
    public HandlerActivationException(Type handlerType) : base($"Failed to activate the handler '{handlerType.FullName}'.") { }
    /// <summary>
    /// Initializes a new instance of the <see cref=HandlerActivationException"/> class with the handler type.
    /// </summary>
    /// <param name="handlerType">The handler type failed to activate.</param>
    /// <param name="innerException">The thrown exception.</param>
    public HandlerActivationException(Type handlerType, Exception innerException) : base($"Failed to activate the handler '{handlerType.FullName}'.", innerException) { }
}