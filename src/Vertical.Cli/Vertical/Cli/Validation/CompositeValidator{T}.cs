namespace Vertical.Cli.Validation;

internal sealed class CompositeValidator<T> : Validator<T>
{
    private readonly IEnumerable<Validator<T>> _validators;

    internal CompositeValidator(IEnumerable<Validator<T>> validators)
    {
        _validators = validators;
    }
    
    /// <inheritdoc />
    public override void Validate(ValidationContext<T> context)
    {
        foreach (var validator in _validators)
        {
            validator.Validate(context);
        }
    }

    /// <inheritdoc />
    public override string ToString() => $"({_validators.Count()})";
}