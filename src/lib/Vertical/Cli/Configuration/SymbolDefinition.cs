using Vertical.Cli.Binding;
using Vertical.Cli.Conversion;
using Vertical.Cli.Utilities;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines an argument, option, switch or other symbol definition.
/// </summary>
public abstract class SymbolDefinition
{
    internal SymbolDefinition(
        SymbolType type,
        ICommandDefinition parent,
        Func<IBinder> binderFactory,
        int position,
        string id,
        string[] aliases,
        Arity arity,
        string? description,
        SymbolScope scope)
    {
        Type = type;
        Parent = parent;
        BinderFactory = binderFactory;
        Position = position;
        Id = id;
        Aliases = aliases;
        Arity = arity;
        Description = description;
        Scope = scope;
    }

    /// <summary>
    /// Gets the symbol type.
    /// </summary>
    public SymbolType Type { get; }

    /// <summary>
    /// Gets the command that this symbol is defined in.
    /// </summary>
    public ICommandDefinition Parent { get; }

    /// <summary>
    /// Gets an <see cref="IBinder"/> provider.
    /// </summary>
    public Func<IBinder> BinderFactory { get; }

    /// <summary>
    /// Gets the position of this symbol relative to the other symbol's in the path.
    /// </summary>
    public int Position { get; }

    /// <summary>
    /// Gets the primary identity.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the aliases.
    /// </summary>
    public string[] Aliases { get; }

    /// <summary>
    /// Gets all of the identities for the symbol.
    /// </summary>
    public IEnumerable<string> Identities => new[] { Id }.Concat(Aliases);

    /// <summary>
    /// Gets the arity.
    /// </summary>
    public Arity Arity { get; }

    /// <summary>
    /// Gets the description of the symbol.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the scope of the symbol within the command path.
    /// </summary>
    public SymbolScope Scope { get; }

    /// <summary>
    /// Gets the binding value type.
    /// </summary>
    public abstract Type ValueType { get; }
}

/// <summary>
/// Defines a strongly-typed symbol.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public sealed class SymbolDefinition<T> : SymbolDefinition where T : notnull
{
    /// <inheritdoc />
    internal SymbolDefinition(
        SymbolType type,
        ICommandDefinition parent,
        Func<IBinder> binderFactory,
        int position,
        string id, 
        string[] aliases, 
        Arity arity,
        string? description,
        SymbolScope scope,
        Func<T>? defaultProvider,
        ValueConverter<T>? converter,
        Validator<T>? validator) 
        : base(type, parent, binderFactory, position, id, aliases, arity, description, scope)
    {
        DefaultProvider = defaultProvider;
        Converter = converter;
        Validator = validator;
    }

    /// <summary>
    /// Gets the default provider.
    /// </summary>
    public Func<T>? DefaultProvider { get; }

    /// <summary>
    /// Gets the converter.
    /// </summary>
    public ValueConverter<T>? Converter { get; }

    /// <summary>
    /// Gets the validator.
    /// </summary>
    public Validator<T>? Validator { get; }

    /// <inheritdoc />
    public override Type ValueType => typeof(T);

    /// <inheritdoc />
    public override string ToString() => this.GetDisplayString();
}