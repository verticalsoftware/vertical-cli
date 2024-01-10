namespace Vertical.Cli.Validation;

/// <summary>
/// Used to format values in validation messages.
/// </summary>
internal readonly record struct DisplayValue<T>(T Value)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return Value is string
            ? $"\"{Value}\""
            : $"{Value}";
    }
}