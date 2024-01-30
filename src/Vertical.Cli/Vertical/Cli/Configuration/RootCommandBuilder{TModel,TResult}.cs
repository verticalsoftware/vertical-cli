using Vertical.Cli.Help;
using Vertical.Cli.Validation;

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
        Func<IHelpFormatter>? formatterProvider = null)
    {
        
        ReplaceSpecialSymbol(new HelpSymbolDefinition(
            this,
            GetInsertPosition(),
            formatterProvider ?? DefaultHelpFormatter.Create,
            id,
            aliases ?? Array.Empty<string>(),
            description));
        
        return this;
    }

    /// <inheritdoc />
    public ICommandBuilder<TModel, TResult> AddResponseFileOption(
        string id = "--silent",
        string[]? aliases = null,
        string description = "Response file to read for unattended input.",
        Func<FileInfo>? defaultProvider = null,
        Validator<FileInfo>? validator = null)
    {
        ReplaceSpecialSymbol(new ResponseFileSymbolDefinition(
            this,
            GetInsertPosition(),
            id,
            aliases ?? Array.Empty<string>(),
            description,
            defaultProvider));

        return this;
    }
}