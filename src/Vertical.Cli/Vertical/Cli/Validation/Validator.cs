using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Validation;

/// <summary>
/// Validates values before being assigned to symbol definitions.
/// </summary>
public abstract class Validator
{
    internal Validator()
    {
    }
    
    /// <summary>
    /// Gets the value type the validator supports.
    /// </summary>
    public abstract Type ValueType { get; }

    /// <summary>
    /// Configures a validator instance.
    /// </summary>
    /// <param name="configure">An action that adds discreet value conditions.</param>
    /// <typeparam name="T">Value type.</typeparam>
    /// <returns>The created validator instance.</returns>
    public static Validator<T> Configure<T>(Action<ValidationBuilder<T>> configure)
    {
        Guard.IsNotNull(configure);

        var builder = new ValidationBuilder<T>();
        configure(builder);

        return builder.Build();
    }
}