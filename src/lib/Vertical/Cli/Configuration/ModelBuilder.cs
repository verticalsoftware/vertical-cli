using System.Linq.Expressions;
using Vertical.Cli.Binding;
using Vertical.Cli.Help;
using Vertical.Cli.Utilities;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;


/// <summary>
/// Used to fluently configure a model type.
/// </summary>
/// <typeparam name="TModel">The model class that represents an application's arguments.</typeparam>
public sealed class ModelBuilder<TModel> where TModel : class
{
    internal ModelConfiguration Configuration { get; }

    internal ModelBuilder(ModelConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// Associates a model property with a position argument input.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="ordinalPosition">
    /// The expected position of the argument in relation to other position arguments.
    /// </param>
    /// <param name="required">A flag indicating whether a value for the argument is required.</param>
    /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <typeparam name="TValue">The value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public ModelBuilder<TModel> ParseArgument<TValue>(
        Expression<Func<TModel, TValue>> expression,
        int ordinalPosition,
        bool required = false,
        Func<TValue>? defaultProvider = null,
        Action<IValidationEventInfo<TModel, TValue>>? validate = null,
        SymbolHelpTopic? helpTopic = null)
    {
        Configuration.AddBindingSource(new CliSymbol<TModel, TValue>(
            expression,
            SymbolKind.PositionArgument,
            ordinalPosition,
            aliases: [],
            arity: required ? Arity.One : Arity.ZeroOrOne,
            defaultProvider,
            helpTopic,
            ValidationHelpers.TryCreateValidationAction(validate),
            self => new ScalarPropertyBinder<TModel, TValue>(self)));
        
        return this;
    } 
    
    /// <summary>
    /// Associates a model property with position argument input that can be specified more than once.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="ordinalPosition">
    /// The expected position of the argument in relation to other position arguments.
    /// </param>
    /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
    /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <typeparam name="TElement">The value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public ModelBuilder<TModel> ParseRepeatableArgument<TElement>(
        Expression<Func<TModel, TElement[]>> expression,
        int ordinalPosition,
        Arity? arity = null,
        Func<TElement[]>? defaultProvider = null,
        Action<IValidationEventInfo<TModel, TElement, TElement[]>>? validate = null,
        SymbolHelpTopic? helpTopic = null)
    {
        Configuration.AddBindingSource(new CliSymbol<TModel, TElement[]>(
            expression,
            SymbolKind.PositionArgument,
            ordinalPosition,
            aliases: [],
            arity: arity ?? Arity.ZeroOrMore,
            defaultProvider,
            helpTopic,
            ValidationHelpers.TryCreateValidationAction(validate),
            self => new CollectionPropertyBinder<TModel, TElement, TElement[]>(self)));
        
        return this;
    }

    /// <summary>
    /// Associates a model property with position argument input that can be specified more than once.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="ordinalPosition">
    /// The expected position of the argument in relation to other position arguments.
    /// </param>
    /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
    /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <typeparam name="TElement">The value type</typeparam>
    /// <typeparam name="TCollection">The property's collection type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    public ModelBuilder<TModel> ParseRepeatableArgument<TElement, TCollection>(
        Expression<Func<TModel, TCollection>> expression,
        int ordinalPosition,
        Arity? arity = null,
        Func<TCollection>? defaultProvider = null,
        Action<IValidationEventInfo<TModel, TElement, TCollection>>? validate = null,
        SymbolHelpTopic? helpTopic = null)
        where TCollection : IEnumerable<TElement>
    {
        Configuration.AddBindingSource(new CliSymbol<TModel, TCollection>(
            expression,
            SymbolKind.PositionArgument,
            ordinalPosition,
            aliases: [],
            arity: arity ?? Arity.ZeroOrMore,
            defaultProvider,
            helpTopic,
            ValidationHelpers.TryCreateValidationAction(validate),
            self => new CollectionPropertyBinder<TModel, TElement, TCollection>(self)));
        
        return this;
    }

    /// <summary>
    /// Associates a model property with a named option input.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="aliases">
    /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
    /// property name.
    /// </param>
    /// <param name="required">A flag indicating whether a value for the argument is required.</param>
    /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <typeparam name="TValue">The value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public ModelBuilder<TModel> ParseOption<TValue>(
        Expression<Func<TModel, TValue>> expression,
        AliasList? aliases = null,
        bool required = false,
        Func<TValue>? defaultProvider = null,
        Action<IValidationEventInfo<TModel, TValue>>? validate = null,
        SymbolHelpTopic? helpTopic = null)
    {
        var bindingName = expression.BindingName;
        
        Configuration.AddBindingSource(new CliSymbol<TModel, TValue>(
            expression,
            SymbolKind.Option,
            ordinalPosition: null,
            (aliases ?? AliasList.Empty).GetValuesOrDefault(bindingName),
            required ? Arity.One : Arity.ZeroOrOne,
            defaultProvider,
            helpTopic,
            ValidationHelpers.TryCreateValidationAction(validate),
            self => new ScalarPropertyBinder<TModel, TValue>(self)));

        return this;
    }
    
    /// <summary>
    /// Associates a model property with named option inputs that can be specified more than once.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="aliases">
    /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
    /// property name.
    /// </param>
    /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
    /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <typeparam name="TElement">The value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public ModelBuilder<TModel> ParseRepeatableOption<TElement>(
        Expression<Func<TModel, TElement[]>> expression,
        AliasList? aliases = null,
        Arity? arity = null,
        Func<TElement[]>? defaultProvider = null,
        Action<IValidationEventInfo<TModel, TElement, TElement[]>>? validate = null,
        SymbolHelpTopic? helpTopic = null)
    {
        var bindingName = expression.BindingName;
        
        Configuration.AddBindingSource(new CliSymbol<TModel, TElement[]>(
            expression,
            SymbolKind.Option, 
            ordinalPosition: null,
            (aliases ?? AliasList.Empty).GetValuesOrDefault(bindingName),
            arity ?? Arity.ZeroOrMore,
            defaultProvider,
            helpTopic,
            ValidationHelpers.TryCreateValidationAction(validate),
            self => new CollectionPropertyBinder<TModel, TElement, TElement[]>(self)));

        return this;
    }
    
    /// <summary>
    /// Associates a model property with named option inputs that can be specified more than once.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="aliases">
    /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
    /// property name.
    /// </param>
    /// <param name="arity">A value that expresses the minimum and maximum uses of the argument.</param>
    /// <param name="defaultProvider">A method that provides the value used if input isn't provided.</param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <typeparam name="TElement">The value type</typeparam>
    /// <typeparam name="TCollection">The property's collection type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    public ModelBuilder<TModel> ParseRepeatableOption<TElement, TCollection>(
        Expression<Func<TModel, TCollection>> expression,
        AliasList? aliases = null,
        Arity? arity = null,
        Func<TCollection>? defaultProvider = null,
        Action<IValidationEventInfo<TModel, TCollection>>? validate = null,
        SymbolHelpTopic? helpTopic = null)
        where TCollection : IEnumerable<TElement>
    {
        var bindingName = expression.BindingName;
        
        Configuration.AddBindingSource(new CliSymbol<TModel, TCollection>(
            expression,
            SymbolKind.Option,
            ordinalPosition: null,
            (aliases ?? AliasList.Empty).GetValuesOrDefault(bindingName),
            arity ?? Arity.ZeroOrMore,
            defaultProvider,
            helpTopic,
            ValidationHelpers.TryCreateValidationAction(validate),
            self => new CollectionPropertyBinder<TModel, TElement, TCollection>(self)));

        return this;
    }
    
    /// <summary>
    /// Associates a boolean model property to a switch input.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="aliases">
    /// One or more GNU option identifiers. When left <c>null</c> an alias is generated using the
    /// property name.
    /// </param>
    /// <param name="validate">A delegate that performs data validation checks.</param>
    /// <param name="helpTopic">The help topic associated with the argument.</param>
    /// <returns>A reference to this instance.</returns>
    public ModelBuilder<TModel> ParseSwitch(
        Expression<Func<TModel, bool>> expression,
        AliasList? aliases = null,
        Action<IValidationEventInfo<TModel, bool>>? validate = null,
        SymbolHelpTopic? helpTopic = null)
    {
        var bindingName = expression.BindingName;
        
        Configuration.AddBindingSource(new CliSymbol<TModel, bool>(
            expression,
            SymbolKind.Switch,
            ordinalPosition: null, 
            (aliases ?? AliasList.Empty).GetValuesOrDefault(bindingName),
            Arity.ZeroOrOne,
            () => false,
            helpTopic,
            ValidationHelpers.TryCreateValidationAction(validate),
            self => new ScalarPropertyBinder<TModel, bool>(self)));

        return this;
    }

    /// <summary>
    /// Sets a preconfigured private value for a model's property.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="value">The static value to map into new model instances.</param>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    public ModelBuilder<TModel> MapStaticValue<TValue>(Expression<Func<TModel, TValue>> expression, TValue value)
    {
        return MapBindingInfoValue(expression, _ => value, "(static value)");
    }

    /// <summary>
    /// Sets a preconfigured private binding value for a model's property.
    /// </summary>
    /// <param name="expression">Expression that identifies the model's property.</param>
    /// <param name="valueProvider">A delegate that provides the value to bind.</param>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    public ModelBuilder<TModel> MapBindingInfoValue<TValue>(
        Expression<Func<TModel, TValue>> expression,
        Func<PropertyBindingInfo, TValue> valueProvider)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(valueProvider);
        
        var bindingSource = new PrivateBindingSource<TModel, TValue>(
            expression.BindingName,
            valueProvider,
            $"Func<PropertyBindingInfo, {typeof(TValue)}>");
        
        Configuration.AddBindingSource(bindingSource);
        return this;
    }

    /// <summary>
    /// Sets a <see cref="TextReader"/> property of model to the console abstraction's
    /// input text reader.
    /// </summary>
    /// <param name="expression">Expression that identifies a model's <see cref="TextReader"/> property.</param>
    /// <returns>A reference to this instance.</returns>
    public ModelBuilder<TModel> MapInputStream(Expression<Func<TModel, TextReader>> expression)
    {
        return MapBindingInfoValue(expression, info => info.ConsoleInput);
    }

    /// <summary>
    /// Establishes the action that creates instances of the model type.
    /// </summary>
    /// <param name="binder">
    /// An action that uses the parse result to build new instances of the model type.
    /// </param>
    /// <returns>A reference to this instance.</returns>
    public ModelBuilder<TModel> SetBinder(ModelBinder<TModel> binder)
    {
        Configuration.SetBinder(binder);
        return this;
    }
    
    private ModelBuilder<TModel> MapBindingInfoValue<TValue>(
        Expression<Func<TModel, TValue>> expression,
        Func<PropertyBindingInfo, TValue> valueProvider,
        string description)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(valueProvider);
        
        var bindingSource = new PrivateBindingSource<TModel, TValue>(
            expression.BindingName,
            valueProvider,
            description);
        
        Configuration.AddBindingSource(bindingSource);
        return this;
    }
}