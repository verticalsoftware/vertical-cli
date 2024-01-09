namespace Vertical.Cli.Binding;

/// <summary>
/// Defines the default value for a type (used by source generator).
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public struct AsyncDefault<T>
{
    public static Task<T> Value => Task.FromResult(default(T)!);
}