namespace Vertical.Cli.Conversion;

/// <summary>
/// Provides a <see cref="ValueConverter{T}"/> implementation for enum types.
/// </summary>
/// <typeparam name="T">Enum type.</typeparam>
public sealed class EnumConverter<T> : ValueConverter<T> where T : struct
{
    /// <inheritdoc />
    public override T Convert(string s)
    {
        return Enum.Parse<T>(s, ignoreCase: true);
    }
}

/// <summary>
/// Defines a nullable variant for the enum type.
/// </summary>
public sealed class NullableEnumConverter<T> : ValueConverter<T?> where T : struct
{
    /// <inheritdoc />
    public override T? Convert(string s)
    {
        return Enum.Parse<T>(s, ignoreCase: true);
    }
}