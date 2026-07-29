using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Diagnostics;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Provides information about a parsed directive.
/// </summary>
public sealed class DirectiveEventInfo
{
    internal DirectiveEventInfo(InvocationContext context, DirectiveSymbol symbol,  ArgumentToken token)
    {
        Context = context;
        Symbol = symbol;
        Token = token;
    }

    /// <summary>
    /// Adds an error ot the context.
    /// </summary>
    /// <param name="error"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void AddError(CommandLineError error)
    {
        Context.AddError(error ?? throw new ArgumentNullException(nameof(error)));
    }

    /// <summary>
    /// Gets the invocation context.
    /// </summary>
    public InvocationContext Context { get; }

    /// <summary>
    /// Gets the symbol declaration.
    /// </summary>
    public DirectiveSymbol Symbol { get; }

    /// <summary>
    /// Gets the directive token that was matched.
    /// </summary>
    public ArgumentToken Token { get; }

    /// <summary>
    /// Gets or sets whether to remove the token from the context after the event
    /// handler returns (defaults to <c>true</c>).
    /// </summary>
    public bool RemoveToken { get; set; } = true;

    /// <inheritdoc />
    public override string ToString() => Token.ToString();
}