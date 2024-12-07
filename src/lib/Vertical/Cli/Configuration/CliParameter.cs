using Vertical.Cli.Binding;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the definition of an argument, option, or switch.
/// </summary>
public abstract class CliParameter : IBindingSource
{
    internal CliParameter(
        int index,
        Type modelType,
        Type valueType,
        SymbolKind symbolKind,
        string bindingName,
        string[] identifiers,
        Arity arity,
        string? helpTag)
    {
        Index = index;
        ModelType = modelType;
        ValueType = valueType;
        SymbolKind = symbolKind;
        BindingName = bindingName;
        Identifiers = identifiers.Select(ArgumentSyntax.Parse).ToArray();
        Arity = arity;
        HelpTag = helpTag;

        if (Identifiers.Length == 0)
        {
            throw new ArgumentException("Identifier required", nameof(identifiers));
        }

        if (symbolKind == SymbolKind.Argument &&
            Identifiers.FirstOrDefault(id => id.PrefixType != OptionPrefixType.None) is { } invalidArgumentSyntax)
        {
            throw new ArgumentException($"Invalid identifier for argument '{invalidArgumentSyntax.Text}'",
                nameof(identifiers));
        }

        if (symbolKind != SymbolKind.Argument &&
            Identifiers.FirstOrDefault(id => id.PrefixType == OptionPrefixType.None) is { } invalidOptionSyntax)
        {
            throw new ArgumentException($"Invalid identifier for option '{invalidOptionSyntax.Text}'",
                nameof(identifiers));
        }
    }
    
    internal int Index { get; }
    
    /// <summary>
    /// Gets the model type.
    /// </summary>
    public Type ModelType { get; }

    /// <summary>
    /// Gets the value type.
    /// </summary>
    public Type ValueType { get; }
    
    /// <summary>
    /// Gets the symbol kind.
    /// </summary>
    public SymbolKind SymbolKind { get; }

    /// <summary>
    /// Gets the binding name derived from the expression used during configuration.
    /// </summary>
    public string BindingName { get; }

    /// <summary>
    /// Gets identifier syntaxes.
    /// </summary>
    public ArgumentSyntax[] Identifiers { get; }

    /// <summary>
    /// Gets a comma separated list of identifiers.
    /// </summary>
    public string IdentifierList => string.Join(", ", Identifiers.Select(id => id.Text));
    
    /// <summary>
    /// Gets the arity.
    /// </summary>
    public Arity Arity { get; }

    /// <summary>
    /// Gets the help tag.
    /// </summary>
    public string? HelpTag { get; }
    
    internal string DisplayName => SymbolKind == SymbolKind.Argument
        ? $"Argument '{BindingName}'"
        : $"Option [{string.Join(", ", Identifiers.Select(id => id.Text))}]";

    /// <inheritdoc />
    public override string ToString() => $"[{string.Join(',', Identifiers.Select(id => id.Text))}] ({ModelType.Name}.{BindingName}={ValueType})";

    /// <inheritdoc />
    public abstract bool TryGetValue(out object? obj);
}


internal sealed class CliParameter<TValue> : CliParameter
{
    /// <inheritdoc />
    public CliParameter(
        int index,
        Type modelType, 
        SymbolKind symbolKind, 
        string bindingName, 
        string[] identifiers,
        Arity arity,
        Func<TValue>? defaultProvider,
        string? helpTag) 
        : base(index, modelType, typeof(TValue), symbolKind, bindingName, identifiers, arity, helpTag)
    {
        this.defaultProvider = defaultProvider;
    }

    private readonly Func<TValue>? defaultProvider;

    /// <inheritdoc />
    public override bool TryGetValue(out object? obj)
    {
        var hasProvider = defaultProvider is not null;

        obj = hasProvider ? defaultProvider!() : null;
        return hasProvider;
    }
}