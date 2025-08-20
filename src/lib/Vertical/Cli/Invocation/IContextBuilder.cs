using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Help;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Represents a collection of symbols.
/// </summary>
public interface IContextBuilder
{
    /// <summary>
    /// Gets the list of symbols.
    /// </summary>
    IEnumerable<ISymbol> Symbols { get; }
    
    /// <summary>
    /// Gets property bindings.
    /// </summary>
    IEnumerable<IPropertyBinding> Bindings { get; }

    /// <summary>
    /// Gets the list of directive help tags.
    /// </summary>
    List<DirectiveHelpTag> DirectiveHelpTags { get; }

    /// <summary>
    /// Adds symbols to the builder.
    /// </summary>
    /// <param name="symbols">Symbols to add.</param>
    void AddSymbols(IEnumerable<ISymbol> symbols);

    /// <summary>
    /// Adds bindings to the builder.
    /// </summary>
    /// <param name="bindings"></param>
    void AddBindings(IEnumerable<IPropertyBinding> bindings);
}