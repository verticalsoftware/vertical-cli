using Vertical.Cli.Binding;

namespace Vertical.Cli.Configuration;

internal sealed class RootCommandBuilder<TModel, TResult> : 
    CommandBuilder<TModel, TResult>,
    IRootCommandBuilder<TModel, TResult> 
    where TModel : class
{
    /// <inheritdoc />
    internal RootCommandBuilder(string id) : base(id)
    {
    }
    
    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> AddHelpOption(
        string id = "--help",
        string[]? aliases = null,
        string description = "Display help content",
        TResult returnValue = default!)
    {
        AddSymbol(new SymbolDefinition<bool>(
            SymbolKind.Switch,
            this,
            () => SwitchBinder.Instance,
            GetInsertPosition(),
            id,
            aliases ?? Array.Empty<string>(),
            Arity.ZeroOrOne,
            description,
            SymbolScope.ParentAndDescendents,
            defaultProvider: null,
            validator: null,
            SymbolSpecialType.HelpOption));
        
        return this;
    }
}