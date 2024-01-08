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
}

public abstract class Validator<T> : Validator where T : notnull
{
    /// <inheritdoc />
    public override Type ValueType => typeof(T);

    public abstract void Validate(ValidationContext<T> context);
}