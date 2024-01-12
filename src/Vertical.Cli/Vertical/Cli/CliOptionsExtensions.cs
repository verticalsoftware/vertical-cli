using CommunityToolkit.Diagnostics;
using Vertical.Cli.Validation;

namespace Vertical.Cli;

using Vertical.Cli.Conversion;

/// <summary>
/// Defines methods for configuration of <see cref="CliOptions"/>.
/// </summary>
public static class CliOptionsExtensions
{
    /// <summary>
    /// Adds a conversion function.
    /// </summary>
    /// <param name="options">The options instance.</param>
    /// <param name="converter">A function that converts the provided string value to <typeparamref name="T"/></param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public static CliOptions AddConverter<T>(
        this CliOptions options,
        Func<string, T> converter)
    {
        Guard.IsNotNull(options);
        Guard.IsNotNull(converter);
        
        options.Converters.Add(new DelegatedConverter<T>(converter));
        return options;
    }
    
    /// <summary>
    /// Adds a conversion function.
    /// </summary>
    /// <param name="options">The options instance.</param>
    /// <param name="converter">A function that converts the string value in the context object
    /// to <typeparamref name="T"/></param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public static CliOptions AddConverter<T>(
        this CliOptions options,
        Func<ConversionContext<T>, T> converter)
    {
        Guard.IsNotNull(options);
        Guard.IsNotNull(converter);
        
        options.Converters.Add(new DelegatedConverter<T>(converter));
        return options;
    }
    
#if NET7_0_OR_GREATER    
    /// <summary>
    /// Adds a converter that leverages <see cref="IParsable{T}"/> 
    /// </summary>
    /// <param name="options">The options instance.</param>
    /// <typeparam name="T">Parsable value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public static CliOptions AddConverter<T>(this CliOptions options) where T : IParsable<T>
    {
        Guard.IsNotNull(options);
        options.Converters.Add(new ParsableConverter<T>());
        return options;
    }
#endif
    
    /// <summary>
    /// Adds a validator built using the configuration delegate.
    /// </summary>
    /// <param name="options">Options instance.</param>
    /// <param name="validator">The validator instance.</param>
    /// <typeparam name="T">Value type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static CliOptions AddValidator<T>(
        this CliOptions options,
        Validator<T> validator)
    {
        Guard.IsNotNull(options);

        options.Validators.Add(validator);
        return options;
    }

    /// <summary>
    /// Adds a validator built using the configuration delegate.
    /// </summary>
    /// <param name="options">Options instance.</param>
    /// <param name="configure">Configuration delegate.</param>
    /// <typeparam name="T">Value type.</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static CliOptions AddValidator<T>(
        this CliOptions options,
        Action<ValidationBuilder<T>> configure)
    {
        Guard.IsNotNull(options);
        Guard.IsNotNull(configure);

        var builder = new ValidationBuilder<T>();
        configure(builder);

        options.Validators.Add(builder.Build());
        return options;
    }
}