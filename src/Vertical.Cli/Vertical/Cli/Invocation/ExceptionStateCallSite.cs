namespace Vertical.Cli.Invocation;

internal static class ExceptionStateCallSite
{
    internal static CallSite<TResult> Create<TResult>(
        Exception exception,
        CliOptions options,
        TResult value)
    {
        return CallSite.Create((None _, CancellationToken _) => HandleExceptionSite(exception, options, value),
            isHelpSite: false);
    }

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