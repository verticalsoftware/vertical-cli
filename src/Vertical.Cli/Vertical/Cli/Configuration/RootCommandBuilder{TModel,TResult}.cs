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
        string? id = null,
        string[]? aliases = null,
        SymbolScope scope = SymbolScope.Parent,
        TResult returnValue = default!)
    {
        AddSymbol(new HelpSymbolDefinition<TResult>(
            this,
            GetInsertPosition(),
            id ?? "--help",
            aliases ?? Array.Empty<string>(),
            scope,
            returnValue));
        
        return this;
    }
}