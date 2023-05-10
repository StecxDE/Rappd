namespace Rappd.CQRS;

/// <summary>
/// Represents an attribute used to decide which handler to use.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public abstract class HandlerSelectionAttribute : Attribute
{
    /// <summary>
    ///  When overridden in a derived class, checks if the atttribute matches the current conditions.
    /// </summary>
    /// <returns><see cref="true"/> if the atttribute matches the current conditions, otherwise <see cref="false"/>.</returns>
    public abstract bool IsMatch();
}
