using System.Linq.Expressions;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Validation;

/// <summary>
/// Used to build one or more validations for a parameter.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
/// <typeparam name="TValue">Value type</typeparam>
public sealed class ValidationBuilder<TModel, TValue> where TModel : class
{
    private readonly CliParameter parameter;
    private readonly Expression<Func<TModel, TValue>> binding;
    private readonly List<ArgumentValidator<TModel>> validators = new(4);

    internal ValidationBuilder(CliParameter parameter, Expression<Func<TModel, TValue>> binding)
    {
        this.parameter = parameter;
        this.binding = binding;
    }
    
    /// <summary>
    /// Adds a validation that is functionally implemented.
    /// </summary>
    /// <param name="evaluator">Function that receives the value and returns whether the value
    /// is valid.</param>
    /// <param name="message">Message to report if validation fails.</param>
    /// <returns>A reference to this instance.</returns>
    public ValidationBuilder<TModel, TValue> Must(Func<TValue, bool> evaluator, string? message = null)
    {
        validators.Add(new ArgumentValidator<TModel, TValue>(parameter,
            message ?? "value invalid",
            binding,
            (_, value) => evaluator(value)));
        return this;
    }

    /// <summary>
    /// Adds a validation that is functionally implemented.
    /// </summary>
    /// <param name="evaluator">Function that receives the model and value and returns whether the value
    /// is valid.</param>
    /// <param name="message">Message to report if validation fails.</param>
    /// <returns>A reference to this instance.</returns>
    public ValidationBuilder<TModel, TValue> Must(Func<TModel, TValue, bool> evaluator, string? message = null)
    {
        validators.Add(new ArgumentValidator<TModel, TValue>(parameter,
            message ?? "value invalid",
            binding,
            evaluator));
        return this;
    }

    internal IEnumerable<ArgumentValidator<TModel>> Build() => validators;
}