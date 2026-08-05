using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Represents data used to handle the invocation of a directive.
/// </summary>
public class DirectiveEventInfo
{
    internal DirectiveEventInfo(
        InvocationContext context,
        ArgumentToken token,
        IDirectiveSymbol symbol)
    {
        Context = context;
        Token = token;
        Symbol = symbol;
    }

    /// <summary>
    /// Gets the invocation context.
    /// </summary>
    public InvocationContext Context { get; }

    /// <summary>
    /// Gets the matched argument token.
    /// </summary>
    public ArgumentToken Token { get; }

    /// <summary>
    /// Gets the symbol reference.
    /// </summary>
    public IDirectiveSymbol Symbol { get; }
}

/// <summary>
/// Represents data used to handle the invocation of a directive.
/// </summary>
/// <typeparam name="TValue">The directive's parameter type.</typeparam>
public sealed class DirectiveEventInfo<TValue> : DirectiveEventInfo
{
    /// <inheritdoc />
    internal DirectiveEventInfo(
        InvocationContext context,
        ArgumentToken token,
        ParameterizedDirectiveSymbol<TValue> symbol,
        TValue value) 
        : base(context, token, symbol)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the parameter value.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Gets the application's property bag.
    /// </summary>
    public OptionsManager ApplicationOptions => Context.ApplicationOptions;
}