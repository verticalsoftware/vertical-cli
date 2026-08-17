using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Middleware.Components;

internal static class HandleMiddlewareSwitchesMiddleware
{
    public static async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
    {
        if (await HandleInternalAsync(context))
            return;
        
        await next(context);
    }

    private static async Task<bool> HandleInternalAsync(InvocationContext context)
    {
        var token = context.TokenList.First;
        if (token is null) return false;

        var switches = context
            .Configuration
            .GetMiddlewareSymbols()
            .Where(symbol => symbol.Kind == SymbolKind.Switch)
            .ToArray();

        if (switches.Length == 0) return false;

        for (; token is { Kind: not TokenKind.OptionsTerminator }; token = token.Next)
        {
            var matchedSwitch = switches.FirstOrDefault(symbol => symbol
                .Aliases
                .Any(alias => alias.Equals(token.Text)));

            if (matchedSwitch is null)
                continue;

            await matchedSwitch.HandleAsync(context, token);
            context.Result = 0;
            return true;
        }

        return false;
    }
}