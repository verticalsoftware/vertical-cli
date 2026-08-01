using Vertical.Cli.Diagnostics;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a directive symbol.
/// </summary>
public sealed class DirectiveSymbol : IDirectiveSymbol
{
    private readonly Func<DirectiveEventInfo, Task> _handler;

    internal DirectiveSymbol(
        string identifier,
        ParameterArity parameterArity,
        Func<DirectiveEventInfo, Task> handler,
        SymbolHelpTopic? helpTopic
    )
    {
        _handler = handler;
        Identifier = identifier;
        ParameterArity = parameterArity;
        HelpTopic = helpTopic;
    }

    /// <summary>
    /// Gets the directive identifier.
    /// </summary>
    public string Identifier { get; }
    
    /// <summary>
    /// Gets the parameter arity.
    /// </summary>
    public ParameterArity ParameterArity { get; }

    /// <summary>
    /// Gets the help topic.
    /// </summary>
    public SymbolHelpTopic? HelpTopic { get; }

    /// <inheritdoc />
    HelpTopic? IHelpSubject.HelpTopic => HelpTopic;

    /// <inheritdoc />
    public SymbolKind Kind => SymbolKind.Directive;

    /// <inheritdoc />
    public Arity Arity => default;

    /// <inheritdoc />
    public async Task HandleAsync(InvocationContext context, ArgumentToken token)
    {
        await _handler(new DirectiveEventInfo(context, token, this));
    }
}

/// <summary>
/// Represents a directive symbol with a parameter value.
/// </summary>
/// <typeparam name="TValue">Parameter value type.</typeparam>
public sealed class ParameterizedDirectiveSymbol<TValue> : IDirectiveSymbol
{
    private readonly Func<DirectiveEventInfo<TValue>, Task> _handler;

    internal ParameterizedDirectiveSymbol(
        string identifier,
        ParameterArity parameterArity,
        Func<DirectiveEventInfo<TValue>, Task> handler,
        Func<TValue>? defaultProvider,
        SymbolHelpTopic? helpTopic)
    {
        Identifier = identifier;
        ParameterArity = parameterArity;
        DefaultProvider = defaultProvider;
        HelpTopic = helpTopic;
        
        _handler = handler;
    }

    public string Identifier { get; }

    public ParameterArity ParameterArity { get; }


    public Func<TValue>? DefaultProvider { get; }

    public SymbolHelpTopic? HelpTopic { get; }

    HelpTopic? IHelpSubject.HelpTopic => HelpTopic;

    /// <inheritdoc />
    public async Task HandleAsync(InvocationContext context, ArgumentToken token)
    {
        switch (token, me: this)
        {
            case { token.Value: null, me.DefaultProvider: not null }:
                await _handler(new DirectiveEventInfo<TValue>(context, token, this, DefaultProvider()));
                break;
                
            case { token.Value: null, me.ParameterArity: ParameterArity.ZeroOrOne }:
                await _handler(new DirectiveEventInfo<TValue>(context, token, this, default!));
                break;
                
            case { token.Value: null }:
                context.AddError(new SymbolArityError(this, []));
                break;
                
            default:
                if (!TryConvertValue(context, token.Value!, out var parameterValue))
                    return;
                await _handler(new DirectiveEventInfo<TValue>(context, token, this, parameterValue));
                break;
        }
    }

    private bool TryConvertValue(InvocationContext context, string parameterValue, out TValue value)
    {
        var converter = context.Configuration.GetArgumentConverter<TValue>();

        try
        {
            value = converter(parameterValue);
            return true;
        }
        catch (Exception exception)
        {
            context.AddError(new ArgumentConversionError(this,
                typeof(TValue),
                parameterValue,
                exception));
            value = default!;
            return false;
        }
    }

    /// <inheritdoc />
    public SymbolKind Kind => SymbolKind.Directive;

    /// <inheritdoc />
    public Arity Arity => ParameterArity == ParameterArity.ZeroOrOne ? Arity.ZeroOrOne : Arity.One;
}