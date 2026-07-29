using Vertical.Cli.Configuration;
using Vertical.Cli.Diagnostics;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Middleware.Components;

internal static class HandleDirectivesMiddleware
{
    public static async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
    {
        var deleteQueue = new Queue<ArgumentToken>();
        var directives = context.Configuration.GetDirectives();
        
        for (var token = context.TokenList.First; token != null; token = token.Next)
        {
            if (token is not { Kind: TokenKind.Directive })
                continue;

            if (directives.FirstOrDefault(entry => entry.Symbol == token.Symbol)
                is not { AsyncHandler: { } asyncHandler} directive)
            {
                continue;
            }

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (directive.Arity)
            {
                case DirectiveParameterArity.NotSupported when token.Value is { Length: > 0 }:
                    context.AddError(new DirectiveArityError(directive, token));
                    break;
                
                case DirectiveParameterArity.Required when token.Value is null:
                    context.AddError(new DirectiveArityError(directive, token));
                    break;
                
                default:
                    var eventInfo = new DirectiveEventInfo(context, directive, token);
                    await asyncHandler(eventInfo);

                    if (!eventInfo.RemoveToken)
                        continue;

                    deleteQueue.Enqueue(token);
                    break;
            }
        }

        while (deleteQueue.TryDequeue(out var token))
        {
            context.TokenList.Remove(token);
        }

        await next(context);
    }
}