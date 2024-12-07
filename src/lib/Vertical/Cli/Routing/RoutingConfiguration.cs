using System.Text.RegularExpressions;
using Vertical.Cli.Binding;

namespace Vertical.Cli.Routing;

internal sealed class RoutingConfiguration : IRoutingConfiguration
{
    private readonly List<RouteDefinition> definitions = [];
    private readonly HashSet<RoutePath> paths = [];

    public void AddDefinition(RouteDefinition route)
    {
        if (!paths.Add(route.Path))
        {
            throw new CliConfigurationException($"Route '{route}' already mapped");
        }

        definitions.Add(route);
    }

    public void AddRoute<TModel>(RoutePath path, AsyncCallSite<TModel>? callSite, string? helpTag)
        where TModel : class
    {
        AddDefinition(new RouteDefinition(
            path,
            typeof(TModel),
            CreateDefaultBindingFactory(callSite),
            isCallable: callSite != null,
            str => path.Match(str).Length,
            helpTag));
    }

    public Router CreateRouter() => new(definitions);

    /// <inheritdoc />
    public IReadOnlyCollection<RouteDefinition> Definitions => definitions;

    private static BindingFactory CreateDefaultBindingFactory<TModel>(AsyncCallSite<TModel>? callSite)
        where TModel : class
    {
        return (application, arguments, route) =>
        {
            var pathWordCount = route.Path.Pattern.Count(char.IsWhiteSpace) + 1;
            var routeArguments = arguments.Skip(pathWordCount).ToArray();

            return BindingContext.Create(application,
                arguments.ToArray(),
                new RouteTarget(routeArguments, route),
                callSite);
        };
    }
}