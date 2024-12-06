using Vertical.Cli.Configuration;
using Vertical.Cli.Routing;

namespace CliDemo;

public static class HelpExtensions
{
    public static string GetHelpTopic(this RouteDefinition route) =>
        HelpResources.ResourceManager.GetString($"{route.Path}") ?? string.Empty;

    public static string GetHelpTopic(this CliParameter parameter) =>
        HelpResources.ResourceManager.GetString($"{parameter.ModelType.Name}.{parameter.BindingName}") ?? string.Empty;
}