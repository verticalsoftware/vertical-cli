namespace Vertical.Cli.Configuration;

/// <summary>
/// Superclass for all symbols.
/// </summary>
public abstract class CliSymbol : CliObject, ICliSymbol
{
    private protected CliSymbol(
        CliCommand command,
        SymbolType type,
        int index,
        string bindingName,
        string[] names,
        Arity arity,
        CliScope scope,
        string? description,
        string? optionGroup,
        string? operandNotation)
        : base(names, description, bindingName)
    {
        Command = command;
        BindingName = bindingName;
        Type = type;
        Index = index;
        Arity = arity;
        Scope = scope;
        OperandSyntax = operandNotation;
        OptionGroup = optionGroup;
    }
    
    /// <summary>
    /// Gets the object's scope.
    /// </summary>
    public CliScope Scope { get; }

    /// <inheritdoc />
    public string? OptionGroup { get; }

    /// <summary>
    /// Gets the operand notation.
    /// </summary>
    public string? OperandSyntax { get; }

    /// <summary>
    /// Gets the command the symbol is assign to.
    /// </summary>
    public CliCommand Command { get; }

    /// <summary>
    /// Gets the model property binding name.
    /// </summary>
    public string BindingName { get; }

    /// <summary>
    /// Gets the symbol type.
    /// </summary>
    public SymbolType Type { get; }

    /// <summary>
    /// Gets the symbol's index.
    /// </summary>
    public int Index { get; }

    /// <inheritdoc />
    public ICliSymbol? ParentSymbol => Command;

    /// <summary>
    /// Gets whether the symbol is named.
    /// </summary>
    public bool HasNames => Names.Length > 0;

    /// <summary>
    /// Gets the symbols arity.
    /// </summary>
    public Arity Arity { get; }

    /// <summary>
    /// Gets the value type.
    /// </summary>
    public abstract Type ValueType { get; }
    
    /// <inheritdoc />
    public override string ToString() => $"[{DebugDisplayNames}] type={ValueType}, arity={Arity}";

    private string DebugDisplayNames => Names.Length > 0 ? string.Join(',', Names) : "positional";
}

/// <summary>
/// Extends the non-generic <see cref="CliSymbol"/> with type information.
/// </summary>
/// <typeparam name="TValue">Value type</typeparam>
public class CliSymbol<TValue> : CliSymbol
{
    internal CliSymbol(
        CliCommand command,
        SymbolType type,
        int index,
        string bindingName,
        string[] names,
        Arity arity,
        CliScope scope,
        Func<TValue>? defaultProvider,
        string? description,
        string? optionGroup,
        string? operandNotation)
    : base(command, type, index, bindingName, names, arity, scope, description, optionGroup, operandNotation)
    {
        DefaultProvider = defaultProvider;
    }

    /// <summary>
    /// Gets the default provider.
    /// </summary>
    public Func<TValue>? DefaultProvider { get; }

    /// <summary>
    /// Gets the symbol value type.
    /// </summary>
    public override Type ValueType => typeof(TValue);
}