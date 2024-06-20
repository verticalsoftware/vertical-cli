using Vertical.Cli.Conversion;

// ReSharper disable once CheckNamespace
namespace Vertical.Cli;

/// <summary>
/// Extensions
/// </summary>
public static class ValueConverterExtensions
{
    /// <summary>
    /// Adds a converter function.
    /// </summary>
    /// <param name="obj">this</param>
    /// <param name="conversion">A function that accepts the string argument and returns to converted value.</param>
    /// <typeparam name="T">Value type</typeparam>
    public static List<ValueConverter> Add<T>(this List<ValueConverter> obj,
        Func<string, T> conversion)
    {
        obj.Add(ValueConverter.Create(conversion));
        return obj;
    }
}