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
        ParameterArity? parameterArity,
        Func<DirectiveEventInfo, Task> handler,
        HelpTopic? helpTopic)
    {
        Identifier = identifier;
        ParameterArity = parameterArity;
        HelpTopic = helpTopic;
        _handler = handler;
    }

    /// <summary>
    /// Gets the directive identifier.
    /// </summary>
    public string Identifier { get; }
    
    /// <summary>
    /// Gets the parameter arity.
    /// </summary>
    public ParameterArity? ParameterArity { get; }

    /// <inheritdoc />
    public Type? ParameterType => null;

    /// <summary>
    /// Gets the help topic.
    /// </summary>
    public HelpTopic? HelpTopic { get; }

    /// <inheritdoc />
    public string? GetRemarks() => HelpTopic?.Remarks;

    /// <inheritdoc />
    public IEnumerable<ExtendedRemarksSection> GetExtendedRemarksSections() => [];

    /// <inheritdoc />
    public string GetListIdentifier() => Identifier;

    /// <inheritdoc />
    public string? GetParameterName() => null;

    /// <inheritdoc />
    HelpTopic? IHelpSubject.HelpTopic => HelpTopic;

    /// <inheritdoc />
    public string DisplayName => $"[{Identifier}]";

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

    /// <inheritdoc/>
    public string Identifier { get; }

    /// <inheritdoc/>
    public ParameterArity? ParameterArity { get; }

    /// <inheritdoc />
    public Type ParameterType => typeof(TValue);

    /// <summary>
    /// Gets the default provider.
    /// </summary>
    public Func<TValue>? DefaultProvider { get; }

    /// <summary>
    /// Gets the help topic.
    /// </summary>
    public SymbolHelpTopic? HelpTopic { get; }

    /// <inheritdoc />
    public string? GetRemarks() => HelpTopic?.Remarks;

    /// <inheritdoc />
    public IEnumerable<ExtendedRemarksSection> GetExtendedRemarksSections() => [];

    /// <inheritdoc />
    public string GetListIdentifier() => Identifier;

    /// <inheritdoc />
    public string GetParameterName() => HelpTopic?.ParameterSyntax ?? "value";

    HelpTopic? IHelpSubject.HelpTopic => HelpTopic;

    /// <inheritdoc />
    public async Task HandleAsync(InvocationContext context, ArgumentToken token)
    {
        var helpProvider = context.Configuration.HelpOptions.HelpProvider;
        
        switch (token, me: this)
        {
            case { token.Value: null, me.DefaultProvider: not null }:
                await _handler(new DirectiveEventInfo<TValue>(context, token, this, DefaultProvider()));
                break;
                
            case { token.Value: null, me.ParameterArity: Configuration.ParameterArity.ZeroOrOne }:
                await _handler(new DirectiveEventInfo<TValue>(context, token, this, default!));
                break;
                
            case { token.Value: null }:
                context.AddError(SymbolArityError.Create(this, [], helpProvider));
                break;
                
            default:
                if (!TryConvertValue(context, token.Value!, helpProvider, out var parameterValue))
                    return;
                await _handler(new DirectiveEventInfo<TValue>(context, token, this, parameterValue));
                break;
        }
    }

    private bool TryConvertValue(InvocationContext context, 
        string parameterValue,
        IHelpProvider helpProvider,
        out TValue value)
    {
        var converter = context.Configuration.GetArgumentConverter<TValue>();

        try
        {
            value = converter(parameterValue);
            return true;
        }
        catch (Exception exception)
        {
            context.AddError(ArgumentConversionError.Create(this,
                typeof(TValue),
                parameterValue,
                helpProvider,
                exception));
            
            value = default!;
            return false;
        }
    }

    /// <inheritdoc />
    public string DisplayName => $"[{Identifier}]";

    /// <inheritdoc />
    public SymbolKind Kind => SymbolKind.Directive;

    /// <inheritdoc />
    public Arity Arity => Configuration.ParameterArity.One == ParameterArity ? Arity.One : Arity.ZeroOrOne;
}