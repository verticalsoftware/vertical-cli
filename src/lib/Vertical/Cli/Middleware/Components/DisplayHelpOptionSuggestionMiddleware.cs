using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;
using Vertical.Cli.IO;

namespace Vertical.Cli.Middleware.Components;

internal static class DisplayHelpOptionSuggestionMiddleware
{
    public static async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
    {
        await next(context);

        if (context.Errors.Count == 0)
            return;
        
        var output = context.OutputWriter;
        var (commandTarget, _) = CommandResolver.GetTarget(context.RootCommand, context.TokenList);
        var description = commandTarget is RootCommand
            ? "application"
            : "command";
        
        output.WriteLine();
        output.WriteLine($"To get help with this {description}, run:");
        output.WriteWhiteSpace(2);
        output.Write(commandTarget.Path, DisplayElement.CommandName);
        output.WriteWhiteSpace();

        var aliasString = string.Join(", ", context.Configuration.HelpOptions.OptionSymbol.Aliases);
        output.WriteLine(aliasString, DisplayElement.Default);
    }
}