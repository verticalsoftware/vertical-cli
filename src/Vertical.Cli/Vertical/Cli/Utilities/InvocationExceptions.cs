using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Utilities;

internal static class InvocationExceptions
{
    public static Exception InvalidSwitchArgument(SymbolDefinition symbol, string argumentValue)
    {
        return new CliInvalidSwitchValueException(
            "{Symbol}: could not convert {AttemptedValue} to a boolean value",
            new Dictionary<string, object>
            {
                ["Symbol"] = symbol.GetDisplayString(),
                ["AttemptedValue"] = argumentValue
            },
            symbol,
            argumentValue);
    }

    public static Exception MinimumArityNotMet(SymbolDefinition symbol, int count)
    {
        return new CliArityException(
            "{Symbol}: Expected {ExpectedCount} argument(s) but received {ActualCount}.",
            new Dictionary<string, object>
            {
                ["Symbol"] = symbol.GetDisplayString(),
                ["ExpectedCount"] = symbol.Arity.MinCount,
                ["ActualCount"] = count
            },
            symbol,
            count);
    }

    public static Exception MaximumArityExceeded(SymbolDefinition symbol, int count)
    {
        return new CliArityException(
            "{Symbol}: Expected no more than {ExpectedCount} argument(s) but received {ActualCount}.",
            new Dictionary<string, object>
            {
                ["Symbol"] = symbol.GetDisplayString(),
                ["ExpectedCount"] = symbol.Arity.MaxCount!.Value,
                ["ActualCount"] = count
            },
            symbol,
            count);
    }

    public static Exception ValueConversionFailed<T>(SymbolDefinition<T> symbol, Exception exception, string value)
    {
        return new CliValueConversionException(
            "{Symbol}: Could not convert \"{AttemptedValue}\" to {Type}.",
            new Dictionary<string, object>
            {
                ["Symbol"] = symbol.GetDisplayString(),
                ["AttemptedValue"] = value,
                ["Type"] = typeof(T)
            },
            symbol,
            value,
            exception);
    }

    public static Exception ValidationFailed<T>(SymbolDefinition<T> symbol, T value, string[] errors)
    {
        return new CliValidationFailedException(
            "{Symbol}: Provided value \"{AttemptedValue}\" is invalid: {FirstError}",
            new Dictionary<string, object>
            {
                ["Symbol"] = symbol.GetDisplayString(),
                ["AttemptedValue"] = value?.ToString() ?? "(null)",
                ["FirstError"] = errors.First()
            },
            symbol,
            value,
            errors);
    }

    public static Exception ValidationFailed<T>(SymbolDefinition<T> symbol, T value, Exception exception)
    {
        return new CliValidationFailedException(
            "{Symbol}: Provided value \"{AttemptedValue}\" is invalid.",
            new Dictionary<string, object>
            {
                ["Symbol"] = symbol.GetDisplayString(),
                ["AttemptedValue"] = value?.ToString() ?? "(null)"
            },
            symbol,
            value,
            Array.Empty<string>(),
            exception);
    }

    public static Exception OptionMissingOperand<T>(SymbolDefinition<T> symbol)
    {
        return new CliMissingOperandException(
            "{Symbol}: Required value not provided.",
            new Dictionary<string, object>
            {
                ["Symbol"] = symbol.GetDisplayString()
            },
            symbol);
    }

    public static Exception InvalidArguments(SemanticArgumentCollection arguments)
    {
        return new CliInvalidArgumentException(
            "Invalid option, argument, or switch: \"{Argument}\".",
            new Dictionary<string, object> { ["Argument"] = arguments.First().ArgumentSyntax.Text },
            arguments.ToArray());
    }
}