using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli;

/// <summary>
/// Entry point for CLI argument parsing and routing.
/// </summary>
public static class CliEngine
{
    /// <summary>
    /// Gets a context that can be used to bind parameters and invoke a call site.
    /// </summary>
    /// <param name="application">Configured <see cref="CliApplication"/></param>
    /// <param name="arguments">Arguments to parse and route</param>
    /// <returns><see cref="BindingContext"/></returns>
    /// <exception cref="CliArgumentException">Argument input is invalid.</exception>
    /// <exception cref="CliConfigurationException">Configuration is invalid.</exception>
    public static BindingContext GetBindingContext(CliApplication application, string[] arguments)
    {
        return GetBindingContext(application, arguments, bindApplicationName: true);
    }
    
    internal static BindingContext GetBindingContext(CliApplication application, string[] arguments,
        bool bindApplicationName)
    {
        var argumentList = new LinkedList<ArgumentSyntax>(ArgumentParser.Parse(arguments));

        if (bindApplicationName)
        {
            argumentList.AddFirst(ArgumentSyntax.Parse(application.Name));
        }

        if (!application.Router.TrySelectRoute(argumentList, out var route))
        {
            throw Exceptions.PathNotFound(argumentList.ToArray());
        }

        if (route.IsCallable)
        {
            return route.CreateBindingContext(application, argumentList, route);
        }
        
        argumentList.RemoveFirst();
        throw Exceptions.PathNotCallable(application, route, argumentList.ToArray());
    }
}