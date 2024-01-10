using Vertical.Cli.Configuration;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Defines the values parsed from semantic arguments that represent the values
/// of a symbol definition. 
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public sealed class ArgumentBinding<T> : ArgumentBinding
{
    internal ArgumentBinding(SymbolDefinition symbol, IEnumerable<T> values)
    {
        Symbol = symbol;
        Values = values;
    }

    /// <inheritdoc />
    public override string BindingId => Symbol.Id;

    /// <summary>
    /// Gets the symbol definition for the binding.
    /// </summary>
    public SymbolDefinition Symbol { get; }

    /// <summary>
    /// Gets the bound values.
    /// </summary>
    public IEnumerable<T> Values { get; }

    /// <inheritdoc />
    public override string ToString() => $"{Symbol.GetShortDisplayString()} = {ValuesString}";

    private string ValuesString => Values.Count() switch
    {
        0 => "(empty)",
        1 => $"\"{Values.Single()}\"",
        _ => string.Join(", ", Values.Select(value => $"\"{value}\""))
    };
}