namespace Vertical.Cli;

/// <summary>
/// Represents an error detected from argument input.
/// </summary>
public enum CliArgumentError
{
    /// <summary>
    /// Indicates the client invoked a path that is not handled.
    /// </summary>
    PathNotCallable,
    
    /// <summary>
    /// Indicates a path was not matched.
    /// </summary>
    PathNotFound,
    
    /// <summary>
    /// Indicates an option or switch identifier was not found.
    /// </summary>
    IdentifierNotFound,
    
    /// <summary>
    /// Indicates arity was violated for a given parameter. 
    /// </summary>
    InvalidArity,
    
    /// <summary>
    /// Indicates value conversion failed.
    /// </summary>
    BadValueConversion,
    
    /// <summary>
    /// Indicates model validation failed.
    /// </summary>
    InvalidModel
}