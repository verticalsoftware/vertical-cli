namespace Vertical.Cli.Conversion;

/// <summary>
/// Converts arguments to enum values.
/// </summary>
/// <typeparam name="TEnum">Enum type</typeparam>
/// <param name="ignoreCase">Whether to ignore the case of the input string</param>
public sealed class NullableEnumConverter<TEnum>(bool ignoreCase = true) : ValueConverter<TEnum?> where TEnum : struct, Enum
{
    /// <inheritdoc />
    public override TEnum? Convert(string str) => Enum.Parse<TEnum>(str, ignoreCase);
}