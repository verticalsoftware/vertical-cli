using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Middleware.Components;

internal static class HandleMiddlewareSwitchesMiddleware
{
    public static async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
    {
        await HandleInternalAsync(context);
        await next(context);
    }

    private static async Task HandleInternalAsync(InvocationContext context)
    {
        var token = context.TokenList.First;
        if (token is null) return;

        var switches = context
            .Configuration
            .GetMiddlewareSymbols()
            .Where(symbol => symbol.Kind == SymbolKind.Switch)
            .ToArray();

        if (switches.Length == 0) return;

        for (; token is { Kind: not TokenKind.OptionsTerminator }; token = token.Next)
        {
            var matchedSwitch = switches.FirstOrDefault(symbol => symbol
                .Aliases
                .Any(alias => alias.Equals(token.Text)));

            if (matchedSwitch is null)
                continue;

            var result = await matchedSwitch.HandleAsync(context, token);
            context.Result = result ?? 0;
            return;
        }
    }
}