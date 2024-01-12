namespace Vertical.Cli.Invocation;

/// <summary>
/// Defines call states.
/// </summary>
public enum CallState
{
    /// <summary>
    /// Indicates the call site is the command handler.
    /// </summary>
    Command,
    
    /// <summary>
    /// Indicates the call site is the help handler.
    /// </summary>
    Help,
    
    /// <summary>
    /// Indicates the call site is an error handler.
    /// </summary>
    Faulted
}