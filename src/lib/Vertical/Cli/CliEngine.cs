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
        var argumentList = BuildArgumentList(arguments);

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

    private static LinkedList<ArgumentSyntax> BuildArgumentList(string[] args)
    {
        var list = new LinkedList<ArgumentSyntax>();
        
        foreach (var arg in args)
        {
            if (!arg.StartsWith('@'))
            {
                list.AddLast(ArgumentSyntax.Parse(arg));
                continue;
            }

            var path = arg[1..];
            ParseResponseFileArguments(list, path);
        }

        return list;
    }

    private static void ParseResponseFileArguments(LinkedList<ArgumentSyntax> list, string path)
    {
        using var reader = GetResponseFileStreamReader(path);

        foreach (var arg in Arguments.ReadAll(reader))
        {
            list.AddLast(ArgumentSyntax.Parse(arg));
        }
    }

    private static StreamReader GetResponseFileStreamReader(string path)
    {
        try
        {
            return new StreamReader(File.OpenRead(path));
        }
        catch (Exception exception)
        {
            throw new CliArgumentException(CliArgumentError.InvalidResponseFile,
                $"Could not read response file {path}.", innerException: exception);
        }
    }
}