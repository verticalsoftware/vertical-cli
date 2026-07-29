using Vertical.Cli.Help;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Middleware.Components;

internal static class HelpSystemMiddleware
{
    public static async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
    {
        var (targetCommand, token) = CommandResolver.GetTarget(context.RootCommand, context.TokenList);
        var helpOptions = context.Configuration.HelpOptions;
        var helpToken = token?.Next ?? context.TokenList.First;
        var aliases = helpOptions.OptionAliases;

        if (helpToken is not null && aliases.Any(alias => alias.Equals(helpToken.Text)))
        {
            HelpSystem.WriteArticle(context.Configuration, targetCommand);
            context.Result = 0;
            return;
        }

        if (!targetCommand.CanCreateCallSite)
        {
            HelpSystem.WriteArticle(context.Configuration, targetCommand);
            context.Result = 0;
            return;
        }

        await next(context);
    }
}