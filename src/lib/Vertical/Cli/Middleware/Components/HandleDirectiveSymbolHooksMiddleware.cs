using Vertical.Cli.Invocation;

namespace Vertical.Cli.Middleware.Components;

internal static class HandleDirectiveSymbolHooksMiddleware
{
    public static async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
    {
        await HandleDirectivesAsync(context);
        await next(context);
    }

    private static async Task HandleDirectivesAsync(InvocationContext context)
    {
        var directives = context.Configuration.GetDirectives();
        if (directives.Count == 0)
            return;

        var matches = context
            .TokenList
            .Select(token => (
                token,
                directive: directives.FirstOrDefault(directive => directive.Identifier.Equals(token.Symbol))))
            .Where(result => result.directive is not null)
            .Select(result => (result.token, directive: result.directive!))
            .ToArray();

        foreach (var (token, directive) in matches)
        {
            await directive.HandleAsync(context, token);
            context.TokenList.Remove(token);
        }
    }
}