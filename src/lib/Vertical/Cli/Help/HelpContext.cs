using Vertical.Cli.Configuration;
using Vertical.Cli.Routing;

namespace Vertical.Cli.Help;

/// <summary>
/// Provides data about the help subject being rendered.
/// </summary>
public sealed class HelpContext
{
    internal HelpContext(CliApplication application,
        RouteDefinition route,
        RouteDefinition[] childRoutes,
        CliParameter[] parameters,
        string helpIdentifier,
        object helpOptionHelpTag)
    {
        Application = application;
        Route = route;
        ChildRoutes = childRoutes;
        Parameters = parameters;
        HelpIdentifier = helpIdentifier;
        HelpOptionHelpTag = helpOptionHelpTag;
        Arguments = parameters.Where(param => param.SymbolKind == SymbolKind.Argument).ToArray();
        Options = parameters.Where(param => param.SymbolKind != SymbolKind.Argument).ToArray();
    }

    /// <summary>
    /// Gets the application instance.
    /// </summary>
    public CliApplication Application { get; }
    
    /// <summary>
    /// Gets the subject route.
    /// </summary>
    public RouteDefinition Route { get; }
    
    /// <summary>
    /// Gets the subject's direct child routes.
    /// </summary>
    public RouteDefinition[] ChildRoutes { get; }
    
    /// <summary>
    /// Gets the mapped parameters.
    /// </summary>
    public CliParameter[] Parameters { get; }
    
    /// <summary>
    /// Gets the argument parameters.
    /// </summary>
    public CliParameter[] Arguments { get; }
    
    /// <summary>
    /// Gets the option parameters.
    /// </summary>
    public CliParameter[] Options { get; }
    
    /// <summary>
    /// Gets the help identifier.
    /// </summary>
    public string HelpIdentifier { get; }
    
    /// <summary>
    /// Gets the help option help tag.
    /// </summary>
    public object HelpOptionHelpTag { get; }
}  