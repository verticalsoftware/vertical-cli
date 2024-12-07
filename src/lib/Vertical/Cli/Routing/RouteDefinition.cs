using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Routing;

/// <summary>
/// Represents a route definition.
/// </summary>
public sealed class RouteDefinition
{
    private readonly BindingFactory bindingFactory;
    private readonly Func<string, int> matcher;

    internal RouteDefinition(
        RoutePath path,
        Type modelType,
        BindingFactory bindingFactory,
        bool isCallable,
        Func<string, int> matcher,
        string? helpTag)
    {
        this.bindingFactory = bindingFactory;
        this.matcher = matcher;
        Path = path;
        ModelType = modelType;
        IsCallable = isCallable;
        HelpTag = helpTag;
    }

    /// <summary>
    /// Gets the route path.
    /// </summary>
    public RoutePath Path { get; }
    
    /// <summary>
    /// Gets the help tag associated with the route.
    /// </summary>
    public string? HelpTag { get; }
    
    /// <summary>
    /// Gets the model type.
    /// </summary>
    public Type ModelType { get; }

    /// <summary>
    /// Gets whether this route has a call site.
    /// </summary>
    public bool IsCallable { get; }

    /// <summary>
    /// Creates the call site.
    /// </summary>
    /// <param name="application">The <see cref="CliApplication"/></param>
    /// <param name="argumentList">Arguments received</param>
    /// <param name="route">The target route</param>
    /// <returns><see cref="BindingContext"/></returns>
    public BindingContext CreateBindingContext(CliApplication application, 
        LinkedList<ArgumentSyntax> argumentList,
        RouteDefinition route) 
        => bindingFactory(application, argumentList, route);

    /// <summary>
    /// Matches this route to the given path.
    /// </summary>
    /// <param name="path">Path to match</param>
    /// <returns>The number of characters matched</returns>
    public int Match(string path) => matcher(path);

    /// <inheritdoc />
    public override string ToString() => Path.ToString();

    /// <inheritdoc />
    public override int GetHashCode() => Path.GetHashCode();
}