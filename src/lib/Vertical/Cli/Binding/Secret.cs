using System.Diagnostics.CodeAnalysis;

namespace Vertical.Cli.Binding;

/// <summary>
/// Wraps a string and overrides its <c>ToString</c> method so that the value is not
/// rendered.
/// </summary>
public readonly struct Secret : IParsable<Secret>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Secret"/> struct.
    /// </summary>
    /// <param name="value"></param>
    public Secret(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the secret value.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => "******";

    /// <inheritdoc />
    public static Secret Parse(string s, IFormatProvider? provider) => new(s);

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Secret result)
    {
        var isMNullOrWhiteSpace = string.IsNullOrWhiteSpace(s);

        result = new Secret(s ?? string.Empty);
        return !isMNullOrWhiteSpace;
    }
}