using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Middleware;

/// <summary>
/// Represents a middleware option.
/// </summary>
public sealed class MiddlewareDirective : MiddlewareSymbol
{
    private readonly Func<InvocationContext, Task> _handler;

    /// <inheritdoc />
    public MiddlewareDirective(
        string identifier, 
        Func<InvocationContext, Task> handler,
        HelpTopic? helpTopic) 
        : base(SymbolKind.Directive, identifier, aliases: [], parameterType: null,  helpTopic)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        ArgumentNullException.ThrowIfNull(handler);
        
        _handler = handler;
    }

    /// <inheritdoc />
    public override async Task<int?> HandleAsync(InvocationContext context, ArgumentToken _)
    {
        await _handler(context);
        return null;
    }
}