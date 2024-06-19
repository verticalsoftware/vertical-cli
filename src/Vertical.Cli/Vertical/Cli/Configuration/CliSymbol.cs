namespace Vertical.Cli.Configuration;

/// <summary>
/// Superclass for all symbols.
/// </summary>
public abstract class CliSymbol : CliObject, ICliSymbol
{
    private protected CliSymbol(
        CliCommand command,
        SymbolType type,
        string bindingName,
        string[] names,
        Arity arity,
        CliScope scope,
        string? description)
        : base(names, description, bindingName)
    {
        Command = command;
        BindingName = bindingName;
        Type = type;
        Arity = arity;
        Scope = scope;
    }
    
    /// <summary>
    /// Gets the object's scope.
    /// </summary>
    public CliScope Scope { get; }

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
        string bindingName,
        string[] names,
        Arity arity,
        CliScope scope,
        Func<TValue>? defaultProvider,
        string? description)
    : base(command, type, bindingName, names, arity, scope, description)
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