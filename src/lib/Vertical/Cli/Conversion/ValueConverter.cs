namespace Vertical.Cli.Conversion;

/// <summary>
/// Defines a function that converts strings to other types.
/// </summary>
/// <typeparam name="TValue">Value type</typeparam>
public delegate TValue ValueConverter<out TValue>(string str);