using System.Linq.Expressions;
using Vertical.Cli.Binding;
using Vertical.Cli.Help;
using Vertical.Cli.Internal;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Static ModelConfiguration class.
/// </summary>
public class ModelConfiguration
{
    private sealed class Factory(IParser parser) : IModelConfigurationFactory
    {
        /// <inheritdoc />
        public ModelConfiguration<TModel> CreateInstance<TModel>() where TModel : class
        {
            return new ModelConfiguration<TModel>(parser);
        }
    }

    internal static IModelConfigurationFactory CreateFactory(IParser parser)
    {
        return new Factory(parser);
    }

    internal virtual void ValidateModel(object obj, List<UsageError> errors)
    {
    }
}

/// <summary>
/// Object to configure the bindings of a model.
/// </summary>
public sealed class ModelConfiguration<TModel> : ModelConfiguration where TModel : class
{
    internal ModelConfiguration(IParser parser)
    {
        _parser = parser;
        _builderConfigurations =
        [
            TryAddModelValidationTarget,
            AddBindings
        ];
    }

    private readonly IParser _parser;
    private readonly List<IPropertyBinding> _propertyBindings = new(16);
    private readonly List<Action<TModel, List<UsageError>>> _validators = [];
    private readonly List<Action<HandlerContextBuilder>> _builderConfigurations;
    
    /// <summary>
    /// Gets the property bindings.
    /// </summary>
    public IReadOnlyList<IPropertyBinding> PropertyBindings => _propertyBindings;

    /// <summary>
    /// Gets the symbols.
    /// </summary>
    public IEnumerable<ISymbol> Symbols => _propertyBindings
        .Where(binding => binding is ISymbol)
        .Cast<ISymbol>();

    /// <summary>
    /// Adds single value argument.
    /// </summary>
    /// <param name="propertyExpression">
    /// Expression that identifies the property in the model type.
    /// </param>
    /// <param name="precedence">
    /// An integer that determine the order in which the parser pairs arguments with this
    /// symbol.
    /// </param>
    /// <param name="name">
    /// The name of the argument (defaults to the upper snake case name of the binding
    /// expression property).
    /// </param>
    /// <param name="arity">The arity to enforce</param>
    /// <param name="helpTag">An application defined help tag</param>
    /// <param name="configureValidation">An action that validates the value after the model is composed.</param>
    /// <param name="setBindingOptions">An action that resolves the value to bind to the model.</param>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public ModelConfiguration<TModel> AddArgument<TValue>(
        Expression<Func<TModel, TValue>> propertyExpression,
        int precedence,
        string? name = null,
        BasicArity? arity = null,
        SymbolHelpTag? helpTag = null,
        Action<ValidationContext<TModel, TValue>>? configureValidation = null,
        Action<PropertyBinder<TValue>>? setBindingOptions = null)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        if (name != null && TokenSyntax.Parse(name) is not { Kind: SyntaxKind.NonDecorated })
        {
            throw new ArgumentException("Invalid command name", nameof(name));
        }
        
        var bindingName = propertyExpression.GetPropertyName();
        var binding = new SymbolBinding<TModel, TValue>(
            typeof(TModel),
            SymbolBehavior.Argument,
            bindingName,
            precedence,
            [name ?? bindingName.ToUpperSnakeCase()],
            (arity ?? BasicArity.One).Arity,
            helpTag,
            setBindingOptions);
        
        _propertyBindings.Add(binding);
        TryAddValidator(binding, propertyExpression, configureValidation);
        
        return this;
    }

    /// <summary>
    /// Adds a multivalued argument that is typically bound to a collection type.
    /// </summary>
    /// <param name="propertyExpression">
    /// Expression that identifies the property in the model type.
    /// </param>
    /// <param name="precedence">
    /// An integer that determine the order in which the parser pairs arguments with this
    /// symbol.
    /// </param>
    /// <param name="name">
    /// The name of the argument (defaults to the upper snake case name of the binding
    /// expression property).
    /// </param>
    /// <param name="arity">The arity to enforce</param>
    /// <param name="helpTag">An application defined help tag</param>
    /// <param name="configureValidation">An action that validates the value after the model is composed.</param>
    /// <param name="setBindingOptions">An action that resolves the value to bind to the model.</param>
    /// <typeparam name="TCollection">Collection type</typeparam>
    /// <returns>A reference to this instance</returns>
    public ModelConfiguration<TModel> AddCollectionArgument<TCollection>(
        Expression<Func<TModel, TCollection>> propertyExpression,
        int precedence,
        string? name = null,
        CollectionArity? arity = null,
        SymbolHelpTag? helpTag = null,
        Action<ValidationContext<TModel, TCollection>>? configureValidation = null,
        Action<PropertyBinder<TCollection>>? setBindingOptions = null)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        if (name != null && TokenSyntax.Parse(name) is not { Kind: SyntaxKind.NonDecorated })
        {
            throw new ArgumentException("Invalid command name", nameof(name));
        }
        
        var bindingName = propertyExpression.GetPropertyName();
        var binding = new SymbolBinding<TModel, TCollection>(
            typeof(TModel),
            SymbolBehavior.Argument,
            bindingName,
            precedence,
            [name ?? bindingName.ToUpperSnakeCase()],
            (arity ?? CollectionArity.OneOrMore).Arity,
            helpTag,
            setBindingOptions);
        
        _propertyBindings.Add(binding);
        TryAddValidator(binding, propertyExpression, configureValidation);
        
        return this;
    }

    /// <summary>
    /// Adds single value option.
    /// </summary>
    /// <param name="propertyExpression">
    /// Expression that identifies the property in the model type.
    /// </param>
    /// <param name="aliases">
    /// One or more prefixed symbol identifiers (defaults to forming an identifier using the
    /// property name and the convention configured in the parser).
    /// </param>
    /// <param name="arity">The arity to enforce</param>
    /// <param name="helpTag">An application defined help tag</param>
    /// <param name="configureValidation">An action that validates the value after the model is composed.</param>
    /// <param name="setBindingOptions">An action that resolves the value to bind to the model.</param>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public ModelConfiguration<TModel> AddOption<TValue>(
        Expression<Func<TModel, TValue>> propertyExpression,
        string[]? aliases = null,
        BasicArity? arity = null,
        SymbolHelpTag? helpTag = null,
        Action<ValidationContext<TModel, TValue>>? configureValidation = null,
        Action<PropertyBinder<TValue>>? setBindingOptions = null)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        ValidateAliases(aliases);
        
        var bindingName = propertyExpression.GetPropertyName();
        var binding = new SymbolBinding<TModel, TValue>(
            typeof(TModel),
            SymbolBehavior.Option,
            bindingName,
            -1,
            aliases ?? [_parser.CreateSymbol(bindingName)],
            (arity ?? BasicArity.ZeroOrOne).Arity,
            helpTag,
            setBindingOptions);
        
        _propertyBindings.Add(binding);
        TryAddValidator(binding, propertyExpression, configureValidation);
        
        return this;
    }

    /// <summary>
    /// Adds single value option.
    /// </summary>
    /// <param name="propertyExpression">
    /// Expression that identifies the property in the model type.
    /// </param>
    /// <param name="aliases">
    /// One or more prefixed symbol identifiers (defaults to forming an identifier using the
    /// property name and the convention configured in the parser).
    /// </param>
    /// <param name="arity">The arity to enforce</param>
    /// <param name="helpTag">An application defined help tag</param>
    /// <param name="configureValidation">An action that validates the value after the model is composed.</param>
    /// <param name="setBindingOptions">An action that resolves the value to bind to the model.</param>
    /// <typeparam name="TCollection">Value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public ModelConfiguration<TModel> AddCollectionOption<TCollection>(
        Expression<Func<TModel, TCollection>> propertyExpression,
        string[]? aliases = null,
        CollectionArity? arity = null,
        SymbolHelpTag? helpTag = null,
        Action<ValidationContext<TModel, TCollection>>? configureValidation = null,
        Action<PropertyBinder<TCollection>>? setBindingOptions = null)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        ValidateAliases(aliases);
        
        var bindingName = propertyExpression.GetPropertyName();
        var binding = new SymbolBinding<TModel, TCollection>(
            typeof(TModel),
            SymbolBehavior.Option,
            bindingName,
            -1,
            aliases ?? [_parser.CreateSymbol(bindingName)],
            (arity ?? CollectionArity.ZeroOrMore).Arity,
            helpTag,
            setBindingOptions);
        
        _propertyBindings.Add(binding);
        TryAddValidator(binding, propertyExpression, configureValidation);
        
        return this;
    }

    /// <summary>
    /// Adds single value option.
    /// </summary>
    /// <param name="propertyExpression">
    /// Expression that identifies the property in the model type.
    /// </param>
    /// <param name="aliases">
    /// One or more prefixed symbol identifiers (defaults to forming an identifier using the
    /// property name and the convention configured in the parser).
    /// </param>
    /// <param name="helpTag">An application defined help tag</param>
    /// <param name="defaultValue">The value to set in the options model when the symbol is not received.</param>
    /// <param name="configureValidation">An action that validates the value after the model is composed.</param>
    /// <returns>A reference to this instance</returns>
    public ModelConfiguration<TModel> AddSwitch(
        Expression<Func<TModel, bool>> propertyExpression,
        string[]? aliases = null,
        SymbolHelpTag? helpTag = null,
        bool defaultValue = false,
        Action<ValidationContext<TModel, bool>>? configureValidation = null)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        ValidateAliases(aliases);
        
        var bindingName = propertyExpression.GetPropertyName();
        var binding = new SymbolBinding<TModel, bool>(
            typeof(TModel),
            SymbolBehavior.Switch,
            bindingName,
            -1,
            aliases ?? [_parser.CreateSymbol(bindingName)],
            Arity.One,
            helpTag,
            args => args.SetDefaultValue(defaultValue)
        );
        
        _propertyBindings.Add(binding);
        TryAddValidator(binding, propertyExpression, configureValidation);
        
        return this;
    }

    /// <summary>
    /// Maps a value obtained from the parse result to a model during activation.
    /// </summary>
    /// <param name="propertyExpression">
    /// Expression that identifies the property in the model type.
    /// </param>
    /// <param name="mapResult">A method that provides the value based on the parse result.</param>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public ModelConfiguration<TModel> BindParseResultValue<TValue>(
        Expression<Func<TModel, TValue>> propertyExpression,
        Action<PropertyBinder<TValue>> mapResult)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        ArgumentNullException.ThrowIfNull(mapResult);
        
        _propertyBindings.Add(new FunctionalValueBinding<TModel, TValue>(
            typeof(TModel),
            propertyExpression.GetPropertyName(),
            mapResult));

        return this;
    }

    /// <summary>
    /// Maps a specific value to a model during activation.
    /// </summary>
    /// <param name="propertyExpression">
    /// Expression that identifies the property in the model type.
    /// </param>
    /// <param name="setValue">A function that provides the value.</param>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public ModelConfiguration<TModel> BindStaticValue<TValue>(
        Expression<Func<TModel, TValue>> propertyExpression,
        Func<TValue> setValue)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        ArgumentNullException.ThrowIfNull(setValue);
                             
        return BindParseResultValue(
            propertyExpression,
            args => args.SetValue(setValue()));
    }

    /// <summary>
    /// Adds the text values of any tokens not paired with other symbols and bindings to a string collection.
    /// </summary>
    /// <param name="propertyExpression">
    /// Expression that identifies the property in the model type.
    /// </param>
    /// <typeparam name="TCollection">Collection type</typeparam>
    /// <returns>A reference to this instance</returns>
    /// <remarks>
    /// By default, pending tokens from the application's input will introduce a parse error
    /// regardless of whether they are mapped using this method unless configured otherwise using
    /// the parse options.
    /// </remarks>
    public ModelConfiguration<TModel> BindPendingTokenValues<TCollection>(
        Expression<Func<TModel, TCollection>> propertyExpression)
        where TCollection : ICollection<string>, new()
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);

        _propertyBindings.Add(new FunctionalValueBinding<TModel, TCollection>(
            typeof(TModel),
            propertyExpression.GetPropertyName(),
            args =>
            {
                var collection = new TCollection();
                foreach (var token in args.ParseResult.PendingTokens)
                {
                    collection.Add(token.Text);
                }
                
                args.SetValue(collection);
            }));
        
        return this;
    }

    /// <summary>
    /// Adds any tokens not paired with other symbols and bindings to a collection.
    /// </summary>
    /// <param name="propertyExpression">
    /// Expression that identifies the property in the model type.
    /// </param>
    /// <typeparam name="TCollection">Collection type</typeparam>
    /// <returns>A reference to this instance</returns>
    /// <remarks>
    /// By default, pending tokens from the application's input will introduce a parse error
    /// regardless of whether they are mapped using this method unless configured otherwise.
    /// </remarks>
    public ModelConfiguration<TModel> BindPendingTokens<TCollection>(
        Expression<Func<TModel, TCollection>> propertyExpression)
        where TCollection : ICollection<Token>, new()
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);

        _propertyBindings.Add(new FunctionalValueBinding<TModel, TCollection>(
            typeof(TModel),
            propertyExpression.GetPropertyName(),
            args =>
            {
                var collection = new TCollection();
                
                foreach (var token in args.ParseResult.PendingTokens)
                {
                    collection.Add(token);
                }
                
                args.SetValue(collection);
            }));
        
        return this;
    }

    /// <summary>
    /// Assigns the defined input stream to a <see cref="TextReader"/> property.
    /// </summary>
    /// <param name="propertyExpression">The text reader property.</param>
    /// <returns>A reference to this instance.</returns>
    public ModelConfiguration<TModel> BindStandardInput(Expression<Func<TModel, TextReader>> propertyExpression)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);
        
        _propertyBindings.Add(new FunctionalValueBinding<TModel, TextReader>(
            typeof(TModel),
            propertyExpression.GetPropertyName(),
            args => args.SetValue(args.BindingContext.InputStream)));

        return this;
    }

    /// <summary>
    /// Adds an action used to bind the model instance.
    /// </summary>
    /// <param name="bindModel">An action that creates a model instance using the parse result.</param>
    /// <returns>A reference to this instance</returns>
    /// <exception cref="ArgumentNullException"><paramref name="bindModel"/> is null.</exception>
    public ModelConfiguration<TModel> UseBinder(ModelBinder<TModel> bindModel)
    {
        ArgumentNullException.ThrowIfNull(bindModel);
        
        _builderConfigurations.Add(builder => builder.AddModelBinder(bindModel));
        return this;
    }

    /// <summary>
    /// Adds an action used to construct an instance of the model type.
    /// </summary>
    /// <param name="constructor">Factory function that activates the model type.</param>
    /// <returns>A reference to this instance</returns>
    public ModelConfiguration<TModel> UseActivator(Func<TModel> constructor)
    {
        ArgumentNullException.ThrowIfNull(constructor);
        
        _builderConfigurations.Add(builder => builder.AddModelActivator(constructor));
        return this;
    }

    internal void ConfigureContext(HandlerContextBuilder builder)
    {
        foreach (var action in Enumerable.Reverse(_builderConfigurations))
        {
            action(builder);
        }
    }

    /// <inheritdoc />
    internal override void ValidateModel(object obj, List<UsageError> errors)
    {
        if (_validators.Count == 0)
            return;

        var model = (TModel)obj;

        foreach (var validator in _validators)
        {
            validator(model, errors);
        }
    }

    private void AddBindings(HandlerContextBuilder builder)
    {
        builder.AddBindings(_propertyBindings);
    }

    private void TryAddModelValidationTarget(HandlerContextBuilder builder)
    {
        if (_validators.Count == 0)
            return;
        
        builder.ValidationTargets.Add(this);
    }

    private void TryAddValidator<TValue>(
        ISymbolBinding symbol,
        Expression<Func<TModel, TValue>> propertyExpression,
        Action<ValidationContext<TModel, TValue>>? evaluator)
    {
        if (evaluator == null)
            return;
        
        _validators.Add(Validate);

        return;

        void Validate(TModel model, List<UsageError> errors)
        {
            var context = new ValidationContext<TModel, TValue>(
                symbol, 
                model, 
                propertyExpression.Compile()(model), errors);

            evaluator(context);
        }
    }

    private static void ValidateAliases(string[]? aliases)
    {
        if (aliases == null)
            return;

        if (aliases.Length == 0)
        {
            throw new ArgumentException("Alias array must contain one value", nameof(aliases));
        }

        foreach (var alias in aliases)
        {
            if (TokenSyntax.Parse(alias) is { Kind: SyntaxKind.PrefixedIdentifier })
                continue;
            
            throw new ArgumentException($"Invalid alias '{alias}' (must be a prefixed identifier)", nameof(aliases));
        }
    }
}