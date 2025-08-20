using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;
using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

internal static class HelpExtensions
{
    public static async Task InvokeHelpAsync(this InvocationContext context,
        ICommand command,
        Func<IConsole, IHelpProvider>? providerFactory)
    {
        var provider = providerFactory?.Invoke(context.Console) ?? DefaultHelpProvider.CreateDefault(context.Console);
        var model = new HelpModel(context, command);

        await provider.RenderHelpAsync(model);
    }
}