using Vertical.Cli.Binding;
using Vertical.Cli.Help;

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
        IHelpRenderer helpRenderer,
        TResult returnValue) 
        : base(
            SymbolType.HelpOption, 
            parent, 
            () => SwitchBinder.Instance, 
            position, 
            id, 
            aliases, 
            Arity.ZeroOrOne, 
            description: null, 
            scope,
            defaultProvider: null,
            converter: null,
            validator: null)
    {
        HelpRenderer = helpRenderer;
        ReturnValue = returnValue;
    }

    /// <summary>
    /// Gets the help renderer.
    /// </summary>
    public IHelpRenderer HelpRenderer { get; }

    /// <summary>
    /// Gets the return value.
    /// </summary>
    public TResult ReturnValue { get; }

    /// <inheritdoc />
    public override Type ValueType => typeof(bool);
}