using Vertical.Cli.Configuration;

namespace Vertical.Cli.Conversion;

/// <summary>
/// Defines the data of a value conversion.
/// </summary>
/// <param name="Symbol">The symbol the value will be mapped to with an argument binding.</param>
/// <param name="Value">The string value to convert.</param>
/// <typeparam name="T">Value type.</typeparam>
public readonly record struct ConversionContext<T>(SymbolDefinition<T> Symbol, string Value);