using Vertical.Cli.Help;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents an unbound symbol for a command.
/// </summary>
public class UnboundCommandSymbol : UnboundSymbol
{
    private readonly Func<InvocationContext, Command, Task> _handler;

    /// <inheritdoc />
    internal UnboundCommandSymbol(
        string identifier, 
        AliasList aliasList, 
        UnboundScope scope,
        Func<InvocationContext, Command, Task> handler, 
        HelpTopic? helpTopic) 
        : base(identifier, aliasList, UnboundSymbolKind.None, scope, helpTopic)
    {
        _handler = handler;
    }

    /// <summary>
    /// Invokes this instance's handler.
    /// </summary>
    /// <param name="context">The invocation context.</param>
    /// <param name="command">The current command context.</param>
    /// <returns>A task that signals when complete.</returns>
    public Task InvokeAsync(InvocationContext context, Command command) => _handler(context, command);
}