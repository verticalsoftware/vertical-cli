using Vertical.Cli.Binding;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines a symbol for a response file option.
/// </summary>
public sealed class ResponseFileSymbolDefinition : SymbolDefinition<FileInfo>
{
    /// <inheritdoc />
    internal ResponseFileSymbolDefinition(
        ICommandDefinition parent, 
        int position,
        string id,
        string[] aliases,
        string? description,
        Func<FileInfo>? defaultProvider) 
        : base(
            SymbolKind.Option, 
            parent, 
            () => OptionBinder<FileInfo?>.Instance, 
            position, 
            id, 
            aliases, 
            Arity.ZeroOrOne, 
            description, 
            SymbolScope.Parent, 
            defaultProvider, 
            validator: Validation.Validator.Configure<FileInfo>(x => x.FileExists()), 
            SymbolSpecialType.ResponseFileOption)
    {
    }
}