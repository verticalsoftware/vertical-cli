namespace Vertical.Cli;

/// <summary>
/// Defines command line events ids.
/// </summary>
public enum CommandLineError
{
    /// <summary>
    /// Indicates a string argument value could not be converted to the expected model property type.
    /// </summary>
    Conversion,
    
    /// <summary>
    /// Indicates an arity requirement for an option or arguments was not met.
    /// </summary>
    Arity,
    
    /// <summary>
    /// Indicates the client provided an argument that was not mapped to the configuration.
    /// </summary>
    UnmappedArgument,
    
    /// <summary>
    /// Indicates the client provided a value that failed validation.
    /// </summary>
    Validation
}