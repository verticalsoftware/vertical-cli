using System.Text;
using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;
using Vertical.Cli.Routing;

namespace Vertical.Cli.Utilities;

internal static class Exceptions
{
    public static CliConfigurationException ConverterNotFound(Type type, CliParameter parameter)
    {
        return new CliConfigurationException($"No ValueConverter instance found for model type: {type}");
    }

    public static CliArgumentException ConversionFailed(
        RouteDefinition route,
        CliParameter parameter,
        ArgumentSyntax argument, 
        IValueConverter converter, 
        Exception innerException)
    {
        return new CliArgumentException(
            CliArgumentError.BadValueConversion,
            $"{parameter.DisplayName}: could not convert '{argument.Text}' to {parameter.ValueType}.",
            route,
            parameter,
            [argument],
            converter,
            innerException);
    }

    public static CliArgumentException OptionValueNotProvided(RouteDefinition route, 
        CliParameter parameter,
        ArgumentSyntax argument)
    {
        return new CliArgumentException(
            CliArgumentError.InvalidArity,
            $"{parameter.DisplayName}: value not provided",
            route,
            parameter,
            [argument]);
    }

    public static CliArgumentException PathNotFound(ArgumentSyntax[] arguments)
    {
        return arguments.Length > 0
            ? new CliArgumentException(CliArgumentError.PathNotFound,
                $"Unknown command or option '{arguments[0].Text}'",
                arguments: arguments)
            : new CliArgumentException(CliArgumentError.PathNotFound, 
                "Default path unhandled",
                arguments: []);
    }

    public static CliArgumentException PathNotCallable(CliApplication application, RouteDefinition route,
        ArgumentSyntax[] arguments)
    {
        if (arguments.Length > 0)
        {
            return new CliArgumentException(CliArgumentError.PathNotFound,
                $"Invalid command '{arguments[0]}'",
                route,
                arguments: arguments);
        }
        
        var message = application.Router.GetChildRoutes(route).Count > 0
            ? $"'{route.Path}' requires a subcommand"
            : $"'{route.Path}' is not callable";
        
        return new CliArgumentException(CliArgumentError.PathNotCallable,
            message,
            route,
            arguments: arguments);
    }

    public static CliArgumentException SwitchValueProvided(
        RouteDefinition route,
        CliParameter parameter,
        ArgumentSyntax argument)
    {
        return new CliArgumentException(
            CliArgumentError.InvalidArity,
            $"{parameter.DisplayName}: switch cannot have an operand",
            route,
            arguments: [argument]);
    }
    
    public static CliArgumentException InvalidMinimumArity(
        RouteDefinition route,
        CliParameter parameter,
        ArgumentSyntax[] arguments)
    {
        var noun = parameter.Arity.MinCount == 1 ? "value" : "values";
        
        return new CliArgumentException(
            CliArgumentError.InvalidArity,
            $"{parameter.DisplayName}: expected {parameter.Arity.MinCount} {noun} but {arguments.Length} provided.",
            route,
            parameter,
            arguments);
    }

    public static CliArgumentException InvalidModel(
        BindingContext context,
        CliParameter parameter,
        string message,
        object? value)
    {
        message = value != null
            ? $"{parameter.DisplayName}: '{value}' - {message}"
            : $"{parameter.DisplayName}: {message}";
        
        return new CliArgumentException(
            CliArgumentError.InvalidModel,
            message,
            context.RouteDefinition,
            parameter);
    }
    
    public static CliArgumentException InvalidMaximumArity(
        RouteDefinition route,
        CliParameter parameter,
        ArgumentSyntax[] arguments)
    {
        var message = parameter.Arity.MaxCount == 1
            ? $"{parameter.DisplayName}: single value expected but {arguments.Length} provided"
            : $"{parameter.DisplayName}: maximum of {parameter.Arity.MaxCount} values accepted but {arguments.Length} provided";

        return new CliArgumentException(
            CliArgumentError.InvalidArity,
            message,
            route,
            parameter,
            arguments);
    }

    public static CliArgumentException IdentifierNotFound(ArgumentSyntax argument)
    {
        return new CliArgumentException(
            CliArgumentError.IdentifierNotFound,
            $"Unknown option or switch '{argument.IdentifierSymbol}'",
            arguments: [argument]);
    }
}