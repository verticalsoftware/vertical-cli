namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines the arity of a directive parameter.
/// </summary>
public enum DirectiveParameterArity
{
    /// <summary>
    /// Indicates parameter values are not supported.
    /// </summary>
    NotSupported,
    
    /// <summary>
    /// Indicates a parameter value is optional.
    /// </summary>
    Optional,
    
    /// <summary>
    /// Indicates a parameter value is required.
    /// </summary>
    Required
}