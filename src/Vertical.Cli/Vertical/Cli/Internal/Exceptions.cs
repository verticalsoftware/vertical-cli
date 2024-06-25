using System.Text;
using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;
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
    public static Exception InvocationFailed<TModel>(RootCommand<TModel> command, string[] arguments)
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
            CommandLineError.ValueConversion,
            $"{path} {FormatSymbol(symbol)}: Could not convert value \"{value}\" to type {typeof(TValue)}",
            path,
            symbol.Command,
            symbol,
            exception);
    }

    internal static Exception MinimumArityNotMet(string path, CliSymbol symbol, int count)
    {
        var use = symbol.Arity.MinCount == 1 ? "value" : "values";
        var verb = count == 1 ? "was" : "were";
        
        return new CommandLineException(
            CommandLineError.Arity,
            $"{path} {FormatSymbol(symbol)}: expects {symbol.Arity.MinCount} {use}, but {count} {verb} provided.",
            path,
            symbol.Command,
            symbol);
    }

    internal static Exception MaximumArityExceeded(string path, CliSymbol symbol, int count)
    {
        var use = symbol.Arity.MaxCount!.Value == 1 ? "value" : "values";
        var verb = count == 1 ? "was" : "were";
        
        return new CommandLineException(
            CommandLineError.Arity,
            $"{path} {FormatSymbol(symbol)}: allows {symbol.Arity.MinCount} {use}, but {count} {verb} provided.",
            path,
            symbol.Command,
            symbol);
    }

    internal static Exception UnmappedArgument(CliCommand command, string path, string unbound)
    {
        return new CommandLineException(
            CommandLineError.UnmappedArgument,
            $"{path}: unknown option, argument, or command '{unbound}'",
            path,
            command);
    }

    private static string FormatSymbol(CliSymbol symbol)
    {
        return symbol.HasNames
            ? $"[option {string.Join(',', symbol.Names)}]"
            : $"[argument '{symbol.BindingName}']";
    }

    internal static Exception ValidationFailed(string path, IEnumerable<ValidationError> errors)
    {
        var error = errors.First();
        var errorMessage = error.Error ?? "value provided is not valid.";
        var message = $"{path}: {FormatSymbol(error.Symbol)}: {errorMessage}";

        return new CommandLineException(
            CommandLineError.Validation,
            message,
            path,
            error.Symbol.Command,
            error.Symbol);
    }

    internal static Exception ResponseFileNotFound(FileInfo file, Stack<ArgumentPreProcessor.Context> stack)
    {
        return new CommandLineException(
            CommandLineError.ResponseFile,
            $"Response file not found: {file.FullName}",
            string.Empty);
    }

    internal static Exception InvalidResponseFileDirective(Stack<ArgumentPreProcessor.Context> stack)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Invalid response file directive");
        foreach (var item in stack)
        {
            sb.AppendLine($"  at {item.File} line {item.Line}");
        }

        return new CommandLineException(
            CommandLineError.ResponseFile,
            sb.ToString(),
            string.Empty);
    }

    internal static Exception NonTerminatedQuote(Stack<ArgumentPreProcessor.Context> stack)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Unterminated double quote");
        foreach (var item in stack)
        {
            sb.AppendLine($"  at {item.File} line {item.Line}");
        }

        return new CommandLineException(
            CommandLineError.ResponseFile,
            sb.ToString(),
            string.Empty);
    }
}