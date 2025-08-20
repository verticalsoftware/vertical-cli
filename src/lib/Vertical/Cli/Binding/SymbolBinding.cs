using Vertical.Cli.Configuration;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents a binding that pairs positional arguments and options to model
/// properties.
/// </summary>
/// <typeparam name="TValue">Value type</typeparam>
/// <typeparam name="TModel">Model type</typeparam>
public sealed class SymbolBinding<TModel, TValue> : IPropertyBinding<TModel, TValue>, ISymbolBinding
    where TModel : class
{
    internal SymbolBinding(
        Type modelType, 
        SymbolBehavior behavior,
        string bindingName,
        int precedence,
        string[] aliases,
        Arity arity,
        object? helpTag,
        Action<PropertyBinder<TModel, TValue>>? setBindingOptions)
    {
        BindingName = bindingName;
        Precedence = precedence;
        ModelType = modelType;
        Behavior = behavior; 
        Arity = arity;
        Aliases = aliases;
        HelpTag = helpTag;
        BindingOptionsAction = setBindingOptions;
    }

    /// <inheritdoc />
    public string BindingName { get; }

    /// <summary>
    /// Gets an integer that determines the parse order for the symbol.
    /// </summary>
    public int Precedence { get; }

    /// <inheritdoc />
    public object? HelpTag { get; }

    /// <inheritdoc />
    public Type ModelType { get; }

    /// <summary>
    /// Gets the symbol's parsing behavior.
    /// </summary>
    public SymbolBehavior Behavior { get; }

    /// <inheritdoc />
    public Type ValueType => typeof(TValue);
    
    /// <summary>
    /// Gets the symbol arity.
    /// </summary>
    public Arity Arity { get; }

    /// <summary>
    /// Gets an action that binds the symbol value.
    /// </summary>
    public Action<PropertyBinder<TModel, TValue>>? BindingOptionsAction { get; }

    /// <summary>
    /// Gets the aliases the symbol is known by.
    /// </summary>
    public string[] Aliases { get; }

    /// <summary>
    /// Gets whether the symbol has a binding action.
    /// </summary>
    public bool HasBindingOptions => BindingOptionsAction != null;

    /// <inheritdoc />
    public bool TryBindValue(PropertyBinder<TModel, TValue> binder)
    {
        BindingOptionsAction?.Invoke(binder);
        return BindingOptionsAction != null;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Behavior} {AliasString}";

    private string AliasString => Aliases.Length == 1 ? Aliases[0] : $"[{string.Join(", ", Aliases)}]";
}