using Vertical.Cli.Parsing;

namespace Vertical.Cli.Routing;

/// <summary>
/// Describes a matched route.
/// </summary>
/// <param name="Arguments">Arguments applicable to the route.</param>
/// <param name="Route">The route definition.</param>
public record RouteTarget(IReadOnlyList<ArgumentSyntax> Arguments, RouteDefinition Route);