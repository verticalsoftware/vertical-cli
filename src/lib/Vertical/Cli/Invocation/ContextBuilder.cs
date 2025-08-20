using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Help;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Represents a symbol builder.
/// </summary>
public class ContextBuilder : IContextBuilder
{
    private readonly List<object> _entries = new(32);

    /// <inheritdoc />
    public IEnumerable<ISymbol> Symbols => _entries.OfType<ISymbol>();

    /// <inheritdoc />
    public IEnumerable<IPropertyBinding> Bindings => _entries.OfType<IPropertyBinding>();

    /// <inheritdoc />
    public List<DirectiveHelpTag> DirectiveHelpTags { get; } = [];

    /// <inheritdoc />
    public void AddSymbols(IEnumerable<ISymbol> symbols) => _entries.AddRange(symbols);

    /// <inheritdoc />
    public void AddBindings(IEnumerable<IPropertyBinding> bindings) => _entries.AddRange(bindings);
}