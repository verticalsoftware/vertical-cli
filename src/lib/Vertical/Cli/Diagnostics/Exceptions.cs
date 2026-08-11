using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Diagnostics;

internal static class Exceptions
{
    public static InvalidOperationException ServiceProviderNotConfigured()
    {
        return new InvalidOperationException(
            """
            Failed to resolve command handler implementation: application service provider not configured.
            -> Resolve this by calling the following on CommandLineApplication:
               app.UseServices(() => /* return IServiceProvider */);
            """);
    }

    public static InvalidOperationException CommandHandlerNotResolved(Type type)
    {
        return new InvalidOperationException(
            $"""
             Service provider could not resolve handler type {type}."
             -> Resolve this by registering the implementation into the application's service
                collection:
                serviceCollection.AddSingleton<IHandler<TOptions>, {type.Name}>();
             """
            );
    }

    public static ArgumentException InvalidCommandName(string name)
    {
        return new ArgumentException(
            $"""
            Invalid command name (cannot match an option, annotation, or directive pattern): '{name}'.
            -> Command name cannot match any of the following other recognized patterns:
               GNU options, e.g.:     -a, --option
               Directives, e.g.:      [log-level]
               Annotations, e.g.:     @arguments.rsp
            """,
            nameof(name));
    }

    public static InvalidOperationException CallSiteNotSupported(Command command)
    {
        return new InvalidOperationException(
            $$"""
             Handler not established for command '{{command.Path}}'.
             -> Resolve this by establishing a handler for the command (or adding sub commands):
                // Use function implementation
                command.SetHandler(async (options, cancellationToken) => {  /* implementation */ });
                
                // Use resolved service implementation
                command.SetHandler(provider => (IHandler<T>)provider.GetService(typeof(IHandler<T>)));
                
                // Use an implementing type
                command.SetHandler<IHandler<T>, Handler<T>>();
             """
            );
    }

    public static ArgumentException CommandAlreadyParented(SubCommand command)
    {
        return new ArgumentException($"Command '{command.Name}' already parented by '{command.Parent?.Path}'.",
            nameof(command));
    }

    public static InvalidOperationException ModelBinderNotConfigured(Type type)
    {
        return new InvalidOperationException(
            $$"""
             Model binder not configured for type {{type}}.
             -> Resolve by adding a model binder.
                app.ConfigureModel<TOptions>(builder => builder.SetBinder(context => { /* return TOptions */ });
                
                Or, use the source generator on an interface type
                [GeneratedBinding]
                public interface IOptions
                {
                    // ..
                } 
             """
            );
    }

    public static ArgumentException EmptyUnboundSymbolAlias(string parameter)
    {
        return new ArgumentException("Unbound symbol must define an alias.", parameter);
    }
}