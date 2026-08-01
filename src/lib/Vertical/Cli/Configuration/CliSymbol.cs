using System.Linq.Expressions;
using System.Reflection;
using Vertical.Cli.Binding;
using Vertical.Cli.Help;
using Vertical.Cli.Utilities;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

public abstract class CliSymbol : IBindingSource, ICliSymbol
{
    protected CliSymbol(
        SymbolKind kind,
        PropertyInfo propertyInfo,
        int ordinalPosition,
        string[] aliases,
        Arity arity,
        SymbolHelpTopic? helpTopic)
    {
        Kind = kind;
        PropertyInfo = propertyInfo;
        OrdinalPosition = ordinalPosition;
        Aliases = aliases;
        Arity = arity;
        HelpTopic = helpTopic;
    }

    /// <summary>
    /// Gets the symbol kind.
    /// </summary>
    public SymbolKind Kind { get; }

    /// <summary>
    /// Gets the binding property info.
    /// </summary>
    public PropertyInfo PropertyInfo { get; }

    /// <summary>
    /// Gets the declaring model type.
    /// </summary>
    public Type ModelType => PropertyInfo.DeclaringType ?? throw new InvalidOperationException();

    /// <inheritdoc />
    public Type ValueType => PropertyInfo.PropertyType;

    /// <summary>
    /// Gets the symbols associated property name to the model.
    /// </summary>
    public string BindingName => PropertyInfo.Name;

    /// <inheritdoc />
    public abstract PropertyBinder CreatePropertyBinder();

    /// <summary>
    /// Gets the parsing order for a positional argument.
    /// </summary>
    public int OrdinalPosition { get; }

    /// <summary>
    /// Gets the aliases of an option or switch symbol.
    /// </summary>
    public string[] Aliases { get; }

    /// <summary>
    /// Gets the arity requirement.
    /// </summary>
    public Arity Arity { get; }
    
    /// <summary>
    /// Gets the help topic associated with the symbol.
    /// </summary>
    public SymbolHelpTopic? HelpTopic { get; }
    
    HelpTopic? IHelpSubject.HelpTopic => HelpTopic;

    /// <summary>
    /// Calls the configured validation for the symbol.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext"/></param>
    public abstract void Validate(ValidationContext context);
}

public sealed class CliSymbol<TModel, TValue> : CliSymbol where TModel : class
{
    private readonly Expression<Func<TModel, TValue>> _expression;
    private readonly Func<CliSymbol<TModel, TValue>, PropertyBinder> _binderFactory;
    private readonly Action<CliSymbol, ValidationContext>? _validate;

    internal CliSymbol(
        Expression<Func<TModel, TValue>> expression,
        SymbolKind kind,
        string bindingName,
        int ordinalPosition,
        string[] aliases,
        Arity arity,
        Func<TValue>? defaultProvider,
        SymbolHelpTopic? helpTopic,
        Action<CliSymbol, ValidationContext>? validate,
        Func<CliSymbol<TModel, TValue>, PropertyBinder> binderFactory)
        : base(kind, expression.PropertyInfo, ordinalPosition, aliases, arity, helpTopic)
    {
        DefaultProvider = defaultProvider;

        _expression = expression;
        _binderFactory = binderFactory;
        _validate = validate;
    }

    internal TValue GetValue(object model) => _expression.Compile()((TModel)model);

    /// <summary>
    /// Gets a delegate that returns the application defined default value.
    /// </summary>
    public Func<TValue>? DefaultProvider { get; }

    /// <inheritdoc />
    public override PropertyBinder CreatePropertyBinder() => _binderFactory(this);

    /// <inheritdoc />
    public override void Validate(ValidationContext context)
    {
        _validate?.Invoke(this, context);
    }
}