using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Invocation;

internal static class ExceptionStateCallSite
{
    internal static ICallSite<TResult> Create<TResult>(
        Exception exception,
        CliOptions options,
        TResult value)
    {
        var proxyCommand = new EmptyCommandDefinition<TResult>(options);

        return CallSite<TResult>.Create(
            proxyCommand,
            (_, _) => HandleExceptionSite(exception, options, value),
            CallState.Faulted);
    }

    [ExcludeFromCodeCoverage]
    private static TResult HandleExceptionSite<TResult>(Exception exception, CliOptions options, TResult value)
    {
        if (options.DisplayExceptions)
        {
            var textColor = Console.ForegroundColor;
            
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.Message);
                Console.WriteLine();
            }
            finally
            {
                Console.ForegroundColor = textColor;
            }
        }

        if (options.ThrowExceptions)
        {
            throw exception;
        }
        
        return value;
    }
}