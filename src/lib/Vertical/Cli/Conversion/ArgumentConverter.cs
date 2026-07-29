namespace Vertical.Cli.Conversion;

/// <summary>
/// Defines a delegate that converts string arguments to other value types.
/// </summary>
/// <typeparam name="TValue">The argument value to convert.</typeparam>
public delegate TValue ArgumentConverter<out TValue>(string argument);