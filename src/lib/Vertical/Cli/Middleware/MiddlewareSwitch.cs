using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Middleware;

/// <summary>
/// Represents a middleware option.
/// </summary>
public sealed class MiddlewareSwitch : MiddlewareSymbol
{
    private readonly Func<InvocationContext, Task<int?>> _handler;

    /// <inheritdoc />
    public MiddlewareSwitch(
        string identifier, 
        string[] aliases, 
        Func<InvocationContext, Task<int?>> handler,
        HelpTopic? helpTopic) 
        : base(SymbolKind.Switch, identifier, aliases, parameterType: null,  helpTopic)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        ArgumentNullException.ThrowIfNull(handler);
        
        _handler = handler;
    }

    /// <inheritdoc />
    public override Task<int?> HandleAsync(InvocationContext context, ArgumentToken _)
    {
        return _handler(context);
    }
}