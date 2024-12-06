namespace Vertical.Cli.Routing;

/// <summary>
/// Represents the routing configuration.
/// </summary>
public interface IRoutingConfiguration
{
    /// <summary>
    /// Gets the route path definition.
    /// </summary>
    IReadOnlyCollection<RouteDefinition> Definitions { get; }
}