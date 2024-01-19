using Vertical.Cli.Binding;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines an argument, option, switch or other symbol definition.
/// </summary>
public abstract class SymbolDefinition
{
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
        SymbolSpecialType specialType = SymbolSpecialType.None)
    {
        Kind = kind;
        Parent = parent;
        BindingProvider = bindingProvider;
        Position = position;
        Id = id;
        Aliases = aliases;
        Arity = arity;
        Description = description;
        Scope = scope;
        SpecialType = specialType;
    }

    private Func<IBinder> BindingProvider { get; }

    /// <summary>
    /// Gets the symbol type.
    /// </summary>
    public SymbolKind Kind { get; }

    /// <summary>
    /// Gets the command that this symbol is defined in.
    /// </summary>
    public ICommandDefinition Parent { get; }

    /// <summary>
    /// Creates an argument binding.
    /// </summary>
    /// <param name="bindingContext">The binding create context.</param>
    /// <returns><see cref="ArgumentBinding"/></returns>
    public ArgumentBinding CreateBinding(IBindingContext bindingContext)
    {
        var binder = BindingProvider();
        return binder.CreateBinding(bindingContext, this);
    }

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
    /// Gets the symbol special type.
    /// </summary>
    public SymbolSpecialType SpecialType { get; }

    /// <summary>
    /// Gets the binding value type.
    /// </summary>
    public abstract Type ValueType { get; }
}