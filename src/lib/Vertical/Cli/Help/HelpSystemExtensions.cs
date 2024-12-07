using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;
using Vertical.Cli.Routing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

/// <summary>
/// Help system extensions
/// </summary>
public static class HelpSystemExtensions
{
    /// <summary>
    /// Invokes help for the given path.
    /// </summary>
    /// <param name="application">Application instance</param>
    /// <param name="path">Path</param>
    /// <returns>Task</returns>
    /// <exception cref="CliConfigurationException"></exception>
    public static async Task<int> InvokeHelpAsync(this CliApplication application, RoutePath path)
    {
        var arguments = $"{path} {application.HelpSwitch}".Split(' ');
        var bindingContext = CliEngine.GetBindingContext(application, arguments, bindApplicationName: false);

        if (await bindingContext.TryInvokeInternalCallSite(CancellationToken.None) is { } result)
            return result;

        throw new CliConfigurationException($"Help path '{path}' undefined");
    }
    
    internal static RouteDefinition Create(
        Func<IHelpProvider>? helpProvider, 
        string identifier, 
        string? helpTag)
    {
        if (ArgumentSyntax.Parse(identifier) is { PrefixType: OptionPrefixType.None })
        {
            throw new ArgumentException($"Invalid help switch identifier '{identifier}'", nameof(identifier));
        }

        var path = new RoutePath($"^.*?{identifier}$");
        
        return new RouteDefinition(
            path,
            typeof(EmptyModel),
            (application, arguments, _) =>
            {
                var router = application.Router;
                var myArguments = arguments.Take(arguments.Count - 1).ToArray();
                
                if (!router.TrySelectRoute(myArguments, out var subjectRoute))
                {
                    throw Exceptions.PathNotFound(arguments.ToArray());
                }

                return BindingContext.Create(
                    application,
                    arguments.ToArray(),
                    new RouteTarget(myArguments, subjectRoute),
                    CreateCallSite(
                        helpProvider?.Invoke() ?? CreateDefaultHelpProvider(), 
                        identifier, 
                        helpTag ?? "Displays help for the current command"));
            },
            isCallable: true,
            str => path.Match(str).Success ? int.MaxValue : 0,
            helpTag);
    }

    private static AsyncCallSite<BindingContext> CreateCallSite(IHelpProvider helpProvider, string identifier, object helpTag)
    {
        return async (model, _) =>
        {
            var router = model.Application.Router;
            
            var helpContext = new HelpContext(
                model.Application,
                model.RouteDefinition,
                router.GetChildRoutes(model.RouteDefinition).ToArray(),
                model.Parameters.ToArray(),
                identifier,
                helpTag);

            await helpProvider.WriteContentAsync(helpContext);
            
            return 0;
        };
    }

    private static CompactFormatProvider CreateDefaultHelpProvider() => new(
        HelpFormattingOptions.Default,
        () => Console.Out,
        Console.WindowWidth - 2);
}