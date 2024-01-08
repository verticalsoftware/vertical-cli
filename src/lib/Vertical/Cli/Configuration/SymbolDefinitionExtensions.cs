using Vertical.Cli.Utilities;

namespace Vertical.Cli.Configuration;

internal static class SymbolDefinitionExtensions
{
    internal static void ValidateArity(this SymbolDefinition symbol, int argumentCount)
    {
        if (argumentCount < symbol.Arity.MinCount)
        {
            throw InvocationExceptions.MinimumArityNotMet(symbol, argumentCount);
        }

        if (symbol.Arity.MaxCount.HasValue && argumentCount > symbol.Arity.MaxCount)
        {
            throw InvocationExceptions.MaximumArityExceeded(symbol, argumentCount);
        }
    }
}