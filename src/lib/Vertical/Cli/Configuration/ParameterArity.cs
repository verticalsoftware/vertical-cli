namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines the arity of a parameter.
/// </summary>
public enum ParameterArity
{
    /// <summary>
    /// Indicates a parameter value is not supported.
    /// </summary>
    Zero,
    
    /// <summary>
    /// Indicates a parameter value is optional.
    /// </summary>
    ZeroOrOne,
    
    /// <summary>
    /// Indicates a parameter value is required.
    /// </summary>
    One
}