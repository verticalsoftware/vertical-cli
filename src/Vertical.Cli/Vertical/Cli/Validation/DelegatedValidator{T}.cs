namespace Vertical.Cli.Validation;

internal sealed class DelegatedValidator<T> : Validator<T>
{
    private readonly Action<ValidationContext<T>> _implementation;
    
    internal DelegatedValidator(Action<ValidationContext<T>> implementation)
    {
        _implementation = implementation;
    }
    
    /// <inheritdoc />
    public override void Validate(ValidationContext<T> context)
    {
        _implementation(context);
    }
}