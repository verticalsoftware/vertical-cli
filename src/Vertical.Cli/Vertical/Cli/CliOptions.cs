using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;
using Vertical.Cli.Binding;
using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.Validation;

namespace Vertical.Cli;

/// <summary>
/// Options used to control parsing, routing, or binding behavior.
/// </summary>
public sealed class CliOptions
{
    private readonly List<ValueConverter> _converters = new();
    private readonly List<Validator> _validators = new();
    private readonly Dictionary<Type, Func<ModelBinder>> _binderRegistrations = new();
    
    /// <summary>
    /// Gets a collection of global value converters.
    /// </summary>
    public IReadOnlyCollection<ValueConverter> Converters => _converters;

    /// <summary>
    /// Gets a collection of validators.
    /// </summary>
    public IReadOnlyCollection<Validator> Validators => _validators;

    /// <summary>
    /// Gets a collection of application defined model binders.
    /// </summary>
    public IReadOnlyCollection<Type> BinderTypes => _binderRegistrations.Keys;

    /// <summary>
    /// Gets whether to automatically display argument exceptions to the console.
    /// </summary>
    public bool DisplayExceptions { get; set; } = true;

    /// <summary>
    /// Gets whether to automatically throw command line exceptions.
    /// </summary>
    public bool ThrowExceptions { get; set; } = false;
    
    /// <summary>
    /// Adds a conversion function.
    /// </summary>
    /// <param name="converter">A function that converts the provided string value to <typeparamref name="T"/></param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public CliOptions AddConverter<T>(
        Func<string, T> converter)
    {
        Guard.IsNotNull(converter);
        
        _converters.Add(new DelegatedConverter<T>(converter));
        return this;
    }
    
    /// <summary>
    /// Adds a conversion function.
    /// </summary>
    /// <param name="converter">A function that converts the string value in the context object
    /// to <typeparamref name="T"/></param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public CliOptions AddConverter<T>(
        Func<ConversionContext<T>, T> converter)
    {
        Guard.IsNotNull(converter);
        
        _converters.Add(new DelegatedConverter<T>(converter));
        return this;
    }
    
#if NET7_0_OR_GREATER    
    /// <summary>
    /// Adds a converter that leverages the <see cref="IParsable{T}"/> imeplementation
    /// for the type. 
    /// </summary>
    /// <typeparam name="T">Parsable value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public CliOptions AddConverter<T>() where T : IParsable<T>
    {
        _converters.Add(new ParsableConverter<T>());
        return this;
    }
#endif
    
    /// <summary>
    /// Adds a validator built using the configuration delegate.
    /// </summary>
    /// <param name="validator">The validator instance.</param>
    /// <typeparam name="T">Value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public CliOptions AddValidator<T>(
        Validator<T> validator)
    {
        Guard.IsNotNull(validator);

        _validators.Add(validator);
        return this;
    }
    
    /// <summary>
    /// Adds a validator built using the configuration delegate.
    /// </summary>
    /// <param name="configure">Configuration delegate.</param>
    /// <typeparam name="T">Value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public CliOptions AddValidator<T>(
        Action<ValidationBuilder<T>> configure)
    {
        Guard.IsNotNull(configure);

        var builder = new ValidationBuilder<T>();
        configure(builder);

        _validators.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Adds a binder registration.
    /// </summary>
    /// <param name="provider">A function that returns the binder to use for the model type.</param>
    /// <typeparam name="T">Model type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    /// <exception cref="InvalidOperationException">A binder for the model type has already been registered.</exception>
    public CliOptions AddBinder<T>(Func<ModelBinder<T>> provider) where T : class
    {
        Guard.IsNotNull(provider);

        if (_binderRegistrations.TryAdd(typeof(T), provider))
            return this;

        throw new InvalidOperationException($"There is already a binder registration for {typeof(T)}.");
    }

    internal bool TryCreateBinder<T>([NotNullWhen(true)] out ModelBinder<T>? binder)
    {
        if (_binderRegistrations.TryGetValue(typeof(T), out var factory))
        {
            binder = (ModelBinder<T>)factory();
            return true;
        }

        binder = null;
        return false;
    }

    internal bool ContainsBinderRegistration(Type type) => _binderRegistrations.ContainsKey(type);
}