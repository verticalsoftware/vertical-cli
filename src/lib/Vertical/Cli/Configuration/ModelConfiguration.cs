using System.Linq.Expressions;
using Vertical.Cli.Binding;
using Vertical.Cli.Utilities;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// 
/// </summary>
public abstract class ModelConfiguration
{
    private readonly CliParameterSet parameters = new();
    private readonly Dictionary<string, IBindingSource> bindingSources = new();
    
    /// <summary>
    /// Gets the model type.
    /// </summary>
    public abstract Type ModelType { get; }

    /// <summary>
    /// Gets the configured parameters.
    /// </summary>
    public IEnumerable<CliParameter> Parameters => parameters;

    internal IReadOnlyDictionary<string, IBindingSource> BindingSources => bindingSources;

    internal abstract void Validate(BindingContext context, object model);
    
    private protected CliParameter AddParameter<TModel, TValue>(SymbolKind symbolKind,
        Expression<Func<TModel, TValue>> binding,
        string[] identifiers, 
        Arity arity,
        Func<TValue>? defaultProvider,
        object? helpTag)
        where TModel : class
    {
        var bindingName = (binding.Body as MemberExpression)!.Member.Name;

        var parameter = new CliParameter<TValue>(
            parameters.Count,
            typeof(TModel),
            symbolKind,
            bindingName,
            identifiers,
            arity,
            defaultProvider,
            helpTag);
        
        if (parameters.TryAdd(parameter))
            return parameter;
        
        var other = parameters[parameter];

        throw new CliConfigurationException($"Conflicting identifiers:\n-> {parameter}\n-> {other}");
    }

    private protected void AddBindingSource<TModel, TValue>(
        Expression<Func<TModel, TValue>> binding,
        IBindingSource bindingSource)
    {
        var bindingName = (binding.Body as MemberExpression)!.Member.Name;
        bindingSources[bindingName] = bindingSource;
    }

    /// <inheritdoc />
    public override string ToString() => $"{ModelType}, parameters={parameters.Count}";
}

/// <summary>
/// Fluent object used to associate parameters with model properties.
/// </summary>
/// <typeparam name="TModel">The model type</typeparam>
public sealed class ModelConfiguration<TModel> : ModelConfiguration where TModel : class
{
    private readonly ModelValidator<TModel> validator = new();
    
    internal ModelConfiguration()
    {
    }

    /// <inheritdoc />
    public override Type ModelType => typeof(TModel);

    /// <inheritdoc />
    internal override void Validate(BindingContext context, object model)
    {
        if (model is not TModel typedModel)
        {
            throw new InvalidOperationException($"Invalid model type '{typeof(TModel)}'");
        }
        
        validator.Validate(typedModel, (parameter, message, value) => Exceptions.InvalidModel(
            context,
            parameter,
            message,
            value));
    }

    /// <summary>
    /// Adds an argument parameter.
    /// </summary>
    /// <param name="binding">An expression used to identify the property to bind to.</param>
    /// <param name="arity">Arity that describes the minimum and maximum number of accepted uses.</param>
    /// <param name="validation">An action used to configure validation rules for argument values.</param>
    /// <param name="helpTag">Data used by a help provider.</param>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public ModelConfiguration<TModel> Argument<TValue>(
        Expression<Func<TModel, TValue>> binding,
        Arity? arity = null,
        Action<ValidationBuilder<TModel, TValue>>? validation = null,

    object? helpTag = null)
    {
        var parameter = AddParameter(
            SymbolKind.Argument,
            binding,
            [$"$arg_{Guid.NewGuid().ToString()[^12..]}"],
            arity ?? Arity.One,
            defaultProvider: null,
            helpTag: helpTag);

        TryConfigureValidation(validation, parameter, binding);

        return this;
    }

    /// <summary>
    /// Adds an option parameter.
    /// </summary>
    /// <param name="binding">An expression used to identify the property to bind to.</param>
    /// <param name="identifiers">One or more short or long form identifiers.</param>
    /// <param name="arity">Arity that describes the minimum and maximum number of accepted uses.</param>
    /// <param name="defaultProvider">A function that provides a default value.</param>
    /// <param name="validation">An action used to configure validation rules for argument values.</param>
    /// <param name="helpTag">Data used by a help provider.</param>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public ModelConfiguration<TModel> Option<TValue>(
        Expression<Func<TModel, TValue>> binding,
        string[] identifiers,
        Arity? arity = null,
        Func<TValue>? defaultProvider = null,
        Action<ValidationBuilder<TModel, TValue>>? validation = null,
        object? helpTag = null)
    {
        var parameter = AddParameter(
            SymbolKind.Option,
            binding,
            identifiers,
            arity ?? Arity.ZeroOrOne,
            defaultProvider,
            helpTag);
        
        TryConfigureValidation(validation, parameter, binding);

        return this;
    }

    /// <summary>
    /// Adds a switch parameter.
    /// </summary>
    /// <param name="binding">An expression used to identify the property to bind to.</param>
    /// <param name="identifiers">One or more short or long form identifiers.</param>
    /// <param name="validation">An action used to configure validation rules for argument values.</param>
    /// <param name="helpTag">Data used by a help provider.</param>
    /// <returns>A reference to this instance.</returns>
    public ModelConfiguration<TModel> Switch(
        Expression<Func<TModel, bool>> binding,
        string[] identifiers,
        Action<ValidationBuilder<TModel, bool>>? validation = null,
        object? helpTag = null)
    {
        var parameter = AddParameter(
            SymbolKind.Switch,
            binding,
            identifiers,
            Arity.ZeroOrOne,
            defaultProvider: null,
            helpTag);
        
        TryConfigureValidation(validation, parameter, binding);

        return this;
    }

    /// <summary>
    /// Provides a value for a model property that is not associated with a CLI parameter. 
    /// </summary>
    /// <param name="binding">An expression used to identify the property to bind to.</param>
    /// <param name="valueProvider">The value to bind to the model</param>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public ModelConfiguration<TModel> ValueBinding<TValue>(
        Expression<Func<TModel, TValue>> binding,
        Func<TValue> valueProvider)
        where TValue : notnull
    {
        AddBindingSource(binding, new WrappedBindingSource(() => valueProvider()));
        return this;
    }

    private void TryConfigureValidation<TValue>(
        Action<ValidationBuilder<TModel, TValue>>? configure,
        CliParameter parameter, 
        Expression<Func<TModel, TValue>> binding)
    {
        if (configure == null)
            return;

        var builder = new ValidationBuilder<TModel, TValue>(parameter, binding);
        configure(builder);
        
        validator.AddRange(builder.Build());
    }
}