using Vertical.Cli.Binding;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Special symbol for help options.
/// </summary>
/// <typeparam name="TResult">Command result type.</typeparam>
public sealed class HelpSymbolDefinition<TResult> : SymbolDefinition<bool>
{
    internal HelpSymbolDefinition(
        ICommandDefinition parent, 
        int position,
        string id,
        string[] aliases,
        SymbolScope scope,
        TResult returnValue,
        string description) 
        : base(
            SymbolKind.Switch, 
            parent, 
            () => SwitchBinder.Instance, 
            position, 
            id, 
            aliases, 
            Arity.ZeroOrOne, 
            description: description, 
            scope,
            defaultProvider: null,
            validator: null,
            specialType: SymbolSpecialType.HelpOption)
    {
        ReturnValue = returnValue;
    }

    /// <summary>
    /// Gets the value to return if help is invoked.
    /// </summary>
    public TResult ReturnValue { get; }

    /// <inheritdoc />
    public override Type ValueType => typeof(bool);
}