using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Routing;

/// <summary>
/// Represents an object that performs route path matching.
/// </summary>
public sealed class Router
{
    internal Router(IReadOnlyList<RouteDefinition> routeDefinitions)
    {
        RouteDefinitions = routeDefinitions;
    }
    
    /// <summary>
    /// Gets the route definitions.
    /// </summary>
    public IReadOnlyList<RouteDefinition> RouteDefinitions { get; }

    /// <summary>
    /// Gets child routes for the given subject.
    /// </summary>
    /// <param name="route">Route definition</param>
    /// <returns>A collection of sub routes.</returns>
    public IReadOnlyCollection<RouteDefinition> GetChildRoutes(RouteDefinition route)
    {
        var parent = route.Path;
        return RouteDefinitions.Where(other => other.Path.IsChildOf(parent)).ToArray();
    }

    /// <summary>
    /// Tries to match a route.
    /// </summary>
    /// <param name="arguments">Parsed argument syntaxes.</param>
    /// <param name="route">When the method returns, and a match is made, a <see cref="RouteTarget"/>
    /// object.</param>
    /// <returns><c>true</c> if the route was matched.</returns>
    public bool TrySelectRoute(IEnumerable<ArgumentSyntax> arguments,
        [NotNullWhen(true)] out RouteDefinition? route)
    {
        route = default;

        var pathString = string.Join(' ', arguments.Select(arg => arg.Text));

        var routes = RouteDefinitions
            .Select(route => (route, match: route.Match(pathString)))
            .Where(t => t.match > 0)
            .ToArray();
            
        route = routes.Length > 0 ? routes.MaxBy(t => t.match).route : null;
            
        return route != null;
    }
}