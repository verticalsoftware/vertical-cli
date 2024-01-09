namespace Vertical.Cli.Binding;

/// <summary>
/// Defines the default value for a type (used by source generator).
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public readonly struct Default<T>
{
    public static T Value => default!;
}