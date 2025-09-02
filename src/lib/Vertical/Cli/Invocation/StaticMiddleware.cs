using Vertical.Cli.Configuration;
using Vertical.Cli.Internal;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Invocation;

internal static class StaticMiddleware
{
    public static IEnumerable<Middleware> GetDelegates() =>
    [
        InvokeCommand
    ];

    private static Middleware InvokeCommand => async (context, next) =>
    {
        var target = context.GetTargetCommand(dequeue: true);

        if (!target.Command.IsInvocationTarget)
        {
            var command = target.Command;
            
            if (command.Commands.Count == 0)
            {
                throw Exceptions.InvalidCommandInvocationPath(command);
            }
            
            context.Errors.Add(new NonInvokableCommandError(command));
            await next(context);
            return;
        }

        var requestBuilder = target.Command.CreateRequestBuilder(
            context.Configuration,
            ModelConfiguration.CreateFactory(context.Parser));

        var request = requestBuilder.Build(context);

        context.ExitCode = await request.GetResultAsync();
    };
}