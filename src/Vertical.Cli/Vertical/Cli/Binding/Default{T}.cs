using System.Diagnostics.CodeAnalysis;

namespace Vertical.Cli.Binding;

/// <summary>
/// Defines the default value for a type (used by source generator).
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
[ExcludeFromCodeCoverage]
public readonly struct DefaultOf<T>
{
    /// <summary>
    /// Returns the default value for T.
    /// </summary>
    public static T Value => default!;

    /// <summary>
    /// Returns the default value for T in a Task.
    /// </summary>
    public static Task<T> TaskValue => Task.FromResult(Value);
}