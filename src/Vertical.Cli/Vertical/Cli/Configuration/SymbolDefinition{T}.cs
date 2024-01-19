using Vertical.Cli.Binding;
using Vertical.Cli.Conversion;
using Vertical.Cli.Utilities;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines a strongly-typed symbol.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public class SymbolDefinition<T> : SymbolDefinition
{
    /// <inheritdoc />
    internal SymbolDefinition(
        SymbolKind kind,
        ICommandDefinition parent,
        Func<IBinder> bindingProvider,
        int position,
        string id, 
        string[] aliases, 
        Arity arity,
        string? description,
        SymbolScope scope,
        Func<T>? defaultProvider,
        Validator<T>? validator,
        SymbolSpecialType specialType = SymbolSpecialType.None) 
        : base(kind, parent, bindingProvider, position, id, aliases, arity, description, scope, specialType)
    {
        DefaultProvider = defaultProvider;
        Validator = validator;
    }

    /// <summary>
    /// Gets the default provider.
    /// </summary>
    public Func<T>? DefaultProvider { get; }

    /// <summary>
    /// Gets the validator.
    /// </summary>
    public Validator<T>? Validator { get; }

    /// <inheritdoc />
    public override Type ValueType => typeof(T);

    /// <inheritdoc />
    public override string ToString() => this.GetDisplayString();
}