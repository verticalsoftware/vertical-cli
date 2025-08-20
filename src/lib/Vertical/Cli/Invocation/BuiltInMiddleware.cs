namespace Vertical.Cli.Invocation;

/// <summary>
/// Defines built-in middleware components.
/// </summary>
public sealed class BuiltInMiddleware
{
    internal readonly Guid Id = Guid.NewGuid();

    /// <summary>
    /// Identifies the middleware that handles exceptions.
    /// </summary>
    public static readonly BuiltInMiddleware HandleExceptions = new();
    
    /// <summary>
    /// Identifies the middleware that displays help when errors are generated..
    /// </summary>
    public static readonly BuiltInMiddleware AutoDisplayHelp = new();
    
    /// <summary>
    /// Identifies the middleware that handles usage errors..
    /// </summary>
    public static readonly BuiltInMiddleware HandleErrors = new();
    
    /// <summary>
    /// Identifies the middleware that handles cancellation.
    /// </summary>
    public static readonly BuiltInMiddleware HandleCancellation = new();
    
    /// <summary>
    /// Identifies the middleware that handles response files.
    /// </summary>
    public static readonly BuiltInMiddleware ResponseFiles = new();
    
    /// <summary>
    /// Identifies the middleware that displays help.
    /// </summary>
    public static readonly BuiltInMiddleware HelpOption = new();
    
    /// <summary>
    /// Identifies the middleware that displays the app version.
    /// </summary>
    public static readonly BuiltInMiddleware VersionOption = new();
}