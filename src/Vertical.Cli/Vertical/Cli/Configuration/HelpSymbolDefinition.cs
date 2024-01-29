using Vertical.Cli.Binding;
using Vertical.Cli.Help;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines a help option symbol.
/// </summary>
public sealed class HelpSymbolDefinition : SymbolDefinition<bool>
{
    /// <inheritdoc />
    internal HelpSymbolDefinition(
        ICommandDefinition parent,
        int position,
        Func<IHelpFormatter> formatterProvider,
        string id,
        string[] aliases,
        string? description) :
        base(
            SymbolKind.Switch,
            parent, 
            () => SwitchBinder.Instance,
            position, 
            id, 
            aliases, 
            Arity.ZeroOrOne, 
            description, 
            SymbolScope.ParentAndDescendents, 
            defaultProvider: null,
            validator: null, 
            SymbolSpecialType.HelpOption)
    {
        FormatterProvider = formatterProvider;
    }

    /// <summary>
    /// Gets the help formatter provider.
    /// </summary>
    public Func<IHelpFormatter> FormatterProvider { get; }
}