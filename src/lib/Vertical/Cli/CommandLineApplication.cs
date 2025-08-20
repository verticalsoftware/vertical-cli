using Vertical.Cli.Invocation;
using Vertical.Cli.IO;

namespace Vertical.Cli;

/// <summary>
/// Represents a composed command line application.
/// </summary>
public sealed class CommandLineApplication
{
    internal CommandLineApplication(CommandLineBuilder builder)
    {
        _builder = builder;
    }

    private readonly CommandLineBuilder _builder;

    /// <summary>
    /// Runs the application, passing control to the framework.
    /// </summary>
    /// <param name="arguments">The arguments to parse and bind.</param>
    /// <param name="console">The console instance to render messages and help content to.</param>
    /// <returns>A task that represents the exit code.</returns>
    public async Task<int> RunAsync(string[] arguments, IConsole? console = null)
    {
        return await InvocationContext.InvokeAsync(_builder, arguments, console ?? new DefaultConsole());
    }
} 