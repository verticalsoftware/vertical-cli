namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines the arity of a parameter.
/// </summary>
public enum ParameterArity
{
    /// <summary>
    /// Indicates a parameter value is optional (provided by a default value).
    /// </summary>
    ZeroOrOne,
    
    /// <summary>
    /// Indicates a parameter value is required.
    /// </summary>
    One
}