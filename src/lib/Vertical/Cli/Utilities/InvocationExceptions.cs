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
        where T : notnull
    {
        return new CliValueConversionException(
            "{Symbol}: Could not convert \"{AttemptedValue}\" to {Type}.",
            new Dictionary<string, object>
            {
                ["Symbol"] = symbol,
                ["AttemptedValue"] = value,
                ["Type"] = typeof(T)
            },
            symbol,
            value,
            exception);
    }

    public static Exception ValidationFailed<T>(SymbolDefinition<T> symbol, T value, string[] toArray) where T : notnull
    {
        throw new NotImplementedException();
    }

    public static Exception ValidationFailed<T>(SymbolDefinition<T> symbol, T value, Exception toArray) where T : notnull
    {
        throw new NotImplementedException();
    }

    public static Exception OptionMissingOperand<T>(SymbolDefinition<T> symbol) where T : notnull
    {
        throw new NotImplementedException();
    }

    public static Exception InvalidArguments(SemanticArgumentCollection bindingPathSemanticArguments)
    {
        throw new NotImplementedException();
    }
}