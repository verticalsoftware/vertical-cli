using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Validation;

/// <summary>
/// Used to define validation rules for a type.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public class ValidationBuilder<T>
{
    private readonly List<Validator<T>> _validators = new(2);

    /// <summary>
    /// Adds a validator that acts as a discreet rule for candidate values.
    /// </summary>
    /// <param name="validator">The validator instance.</param>
    /// <returns>A reference to this instance.</returns>
    public ValidationBuilder<T> AddValidator(Validator<T> validator)
    {
        Guard.IsNotNull(validator);
        
        _validators.Add(validator);
        return this;
    }

    /// <summary>
    /// Adds a validator that uses a predicate to determine if the candidate argument is valid.
    /// </summary>
    /// <param name="predicate">A predicate that returns whether the value is valid.</param>
    /// <param name="errorMessage">A function that returns the error message to display. If <c>null</c>, a
    /// default message is used.</param>
    /// <returns>A reference to this instance.</returns>
    public ValidationBuilder<T> Must(Func<T, bool> predicate, Func<string>? errorMessage = null)
    {
        Guard.IsNotNull(predicate);
        
        _validators.Add(new DelegatedValidator<T>(context =>
        {
            if (predicate(context.Value))
                return;
            
            context.AddError(errorMessage?.Invoke() ?? "Value is invalid.");
        }));
        return this;
    }
    
    /// <summary>
    /// Constructs the validator instance.
    /// </summary>
    /// <returns><see cref="Validator{T}"/></returns>
    /// <exception cref="InvalidOperationException">Validation rules were not defined.</exception>
    public Validator<T> Build()
    {
        if (_validators.Count == 0)
        {
            throw new InvalidOperationException("No validation rules have been defined.");
        }

        return new CompositeValidator<T>(_validators);
    }
}