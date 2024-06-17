using System.Text;
using Vertical.Cli.Configuration;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Internal;

/// <summary>
/// Defines exception factories.
/// </summary>
public static class Exceptions
{
    /// <summary>
    /// Creates an exception suitable to indicate that a command was not matched.
    /// </summary>
    /// <param name="command">Command</param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static Exception InvocationFailed<TModel, TResult>(RootCommand<TModel, TResult> command, string[] arguments)
        where TModel : class
    {
        return new Exception();
    }

    internal static Exception NoDefaultConverter<TValue>()
    {
        return new InvalidOperationException(
            $"No ValueConverter is available for type {typeof(TValue)}.");
    }

    internal static Exception ConversionFailed<TValue>(
        string path,
        CliSymbol symbol,
        string value,
        Exception exception)
    {
        return new CommandLineException(
            $"{path} {FormatSymbol(symbol)}: Could not convert value \"{value}\" to type {typeof(TValue)}",
            path,
            symbol,
            exception);
    }

    internal static Exception MinimumArityNotMet(string path, CliSymbol symbol, int count)
    {
        var use = symbol.Arity.MinCount == 1 ? "value" : "values";
        var verb = count == 1 ? "was" : "were";
        
        return new CommandLineException(
            $"{path} {FormatSymbol(symbol)}: expects {symbol.Arity.MinCount} {use}, but {count} {verb} provided.",
            path,
            symbol);
    }

    internal static Exception MaximumArityExceeded(string path, CliSymbol symbol, int count)
    {
        var use = symbol.Arity.MaxCount!.Value == 1 ? "value" : "values";
        var verb = count == 1 ? "was" : "were";
        
        return new CommandLineException(
            $"{path} {FormatSymbol(symbol)}: allows {symbol.Arity.MinCount} {use}, but {count} {verb} provided.",
            path,
            symbol);
    }

    internal static Exception UnmappedArgument(string path, string unbound)
    {
        return new CommandLineException(
            $"{path}: unknown option, argument, or symbol '{unbound}'",
            path);
    }

    private static string FormatSymbol(CliSymbol symbol)
    {
        return symbol.HasNames
            ? $"[option {string.Join(',', symbol.Names)}]"
            : $"[argument '{symbol.BindingName}']";
    }

    internal static Exception ValidationFailed(string path,  IEnumerable<ValidationError> errors)
    {
        var sb = new StringBuilder();
        var count = 0;
        sb.AppendLine($"{path}: One or more arguments are invalid:");
        
        foreach (var error in errors)
        {
            if (count++ > 0) sb.AppendLine();
            var message = error.Error ?? "Provided value is invalid.";
            sb.Append($" -> {FormatSymbol(error.Symbol)}: {message}");
        }

        return new CommandLineException(sb.ToString(), path);
    }
}