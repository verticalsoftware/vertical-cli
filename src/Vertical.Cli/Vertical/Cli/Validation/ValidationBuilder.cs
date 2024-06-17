using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Validation;

/// <summary>
/// Fluent builder object for validators.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
/// <typeparam name="TValue">Value type</typeparam>
public sealed class ValidationBuilder<TModel, TValue>
{
    private readonly List<Func<TModel, TValue, ValidationResult>> _evaluators = [];

    /// <summary>
    /// Adds an evaluator to the builder.
    /// </summary>
    /// <param name="evaluator">Function that receives the model and candidate value and returns
    /// a <see cref="ValidationResult"/>.</param>
    /// <returns>A reference to this instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="evaluator"/> is null.</exception>
    public ValidationBuilder<TModel, TValue> Use(Func<TModel, TValue, ValidationResult> evaluator)
    {
        _evaluators.Add(evaluator ?? throw new ArgumentNullException(nameof(evaluator)));
        return this;
    }

    /// <summary>
    /// Configures a validation builder for the element of an array or the generic type of a collection.
    /// </summary>
    /// <param name="validator">A delegate that configures validation for the element type.</param>
    /// <typeparam name="TElement">Element type.</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"><typeparamref name="TElement"/> is not enumerable.</exception>
    public ValidationBuilder<TModel, TValue> Each<TElement>(Action<ValidationBuilder<TModel, TElement>> validator)
    {
        Guard.IsNotNull(validator);
        
        if (!typeof(TValue).IsAssignableTo(typeof(IEnumerable<TElement>)))
        {
            throw new ArgumentException($"{typeof(TValue)} is not enumerable, Each<TElement> cannot be used.");
        }
        
        var builder = new ValidationBuilder<TModel, TElement>();
        validator(builder);

        return Use((model, value) =>
        {
            if (value is not IEnumerable<TElement> values)
                return ValidationResult.Ok;

            var elementEvaluator = builder.Build();
            return values
                       .Select(element => elementEvaluator(model, element))
                       .FirstOrDefault(result => !result.IsValid)
                   ?? ValidationResult.Ok;
        });
    }
    
    /// <summary>
    /// Builds a function that aggregates the evaluators added to this instance.
    /// </summary>
    /// <returns></returns>
    public Func<TModel, TValue, ValidationResult> Build()
    {
        return (model, value) => _evaluators
                                     .Select(eval => eval(model, value))
                                     .FirstOrDefault(result => !result.IsValid)
                                 ?? ValidationResult.Ok;
    }
}