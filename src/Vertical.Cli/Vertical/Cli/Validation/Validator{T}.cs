namespace Vertical.Cli.Validation;

/// <summary>
/// Validates values of a specific type.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Validator<T> : Validator
{
    /// <inheritdoc />
    public override Type ValueType => typeof(T);

    /// <summary>
    /// Performs validation of a value before it is bound with a symbol.
    /// </summary>
    /// <param name="context">Data that describes the symbol and value.</param>
    public abstract void Validate(ValidationContext<T> context);
}