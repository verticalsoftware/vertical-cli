using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;
using Vertical.Cli.Routing;

namespace Vertical.Cli;

/// <summary>
/// Thrown when errors are detected in argument input.
/// </summary>
/// <param name="error">The error code</param>
/// <param name="message">A message that describes the error</param>
/// <param name="route">The matched route</param>
/// <param name="parameter">The associated parameter</param>
/// <param name="arguments">The argument(s) that caused the error</param>
/// <param name="converter">The converter used</param>
/// <param name="innerException">The exception that caused this occurrence</param>
public sealed class CliArgumentException(
    CliArgumentError error, 
    string message,
    RouteDefinition? route = null,
    CliParameter? parameter = null,
    ArgumentSyntax[]? arguments = null,
    IValueConverter? converter = null,
    Exception? innerException = null)
    : Exception(message, innerException)
{
    /// <summary>
    /// Gets the error code
    /// </summary>
    public CliArgumentError Error { get; } = error;

    /// <summary>
    /// The matched route, or <c>null</c> if one was not matched.
    /// </summary>
    public RouteDefinition? Route { get; } = route;

    /// <summary>
    /// Gets the associated parameter
    /// </summary>
    public CliParameter? Parameter { get; } = parameter;
    
    /// <summary>
    /// Gets the argument whose value caused the error.
    /// </summary>
    public ArgumentSyntax[]? Arguments { get; } = arguments;
    
    /// <summary>
    /// Gets the converter instance
    /// </summary>
    public IValueConverter? Converter { get; } = converter;
}