using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Middleware;

/// <summary>
/// Represents a symbol handled by middleware.
/// </summary>
public abstract class MiddlewareSymbol : ICliSymbol
{
    internal MiddlewareSymbol(
        SymbolKind kind,
        string identifier,
        string[] aliases,
        Type? parameterType,
        HelpTopic? helpTopic)
    {
        Kind = kind;
        Identifier = identifier;
        Aliases = aliases;
        ParameterType = parameterType;
        HelpTopic = helpTopic;
    }

    /// <summary>
    /// Invokes the symbol's handler delegate.
    /// </summary>
    /// <param name="context">The invocation context.</param>
    /// <param name="token">Token matched to the symbol.</param>
    /// <returns>A task that provides an integer result on completion.</returns>
    public abstract Task HandleAsync(InvocationContext context, ArgumentToken token);
    
    /// <inheritdoc />
    public HelpTopic? HelpTopic { get; }

    /// <inheritdoc />
    public HelpTopicKey HelpTopicKey => new("symbol", $"({Kind}).{Identifier}");

    /// <inheritdoc />
    public string? GetRemarks() => HelpTopic?.Remarks;

    /// <inheritdoc />
    public IEnumerable<ExtendedRemarksSection> GetExtendedRemarksSections() => [];

    /// <inheritdoc />
    public string GetListIdentifier() => Kind == SymbolKind.Directive
        ? Identifier
        : string.Join(", ", Aliases);

    /// <inheritdoc />
    public string? GetParameterName() => (HelpTopic as SymbolHelpTopic)?.ParameterSyntax ?? DefaultParameterName;

    private string? DefaultParameterName => Kind == SymbolKind.Directive
        ? "value"
        : null;

    /// <inheritdoc />
    public virtual ParameterArity? ParameterArity => null;

    /// <inheritdoc />
    public string DisplayName => GetListIdentifier();

    /// <inheritdoc />
    public SymbolKind Kind { get; }

    /// <inheritdoc />
    public SystemKind SystemKind { get; internal set; } = SystemKind.None;

    /// <summary>
    /// Gets the help topic identifier.
    /// </summary>
    public string Identifier { get; }
    
    /// <summary>
    /// Gets the alias of option symbols.
    /// </summary>
    public string[] Aliases { get; }

    /// <summary>
    /// Gets the parameter type.
    /// </summary>
    public Type? ParameterType { get; }

    /// <inheritdoc />
    public Arity Arity => Arity.ZeroOrOne;

    /// <inheritdoc />
    public override string ToString() => Identifier;
}