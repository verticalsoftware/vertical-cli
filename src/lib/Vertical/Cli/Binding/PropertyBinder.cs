using Vertical.Cli.Configuration;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents functionality that creates binding results.
/// </summary>
public abstract class PropertyBinder
{
    internal PropertyBinder(CliSymbol? symbol = null)
    {
        Symbol = symbol;
    }

    /// <summary>
    /// Gets the symbol if one is associated with this binding.
    /// </summary>
    public CliSymbol? Symbol { get; }

    /// <summary>
    /// Creates the binding result.
    /// </summary>
    /// <param name="bindingInfo">An object that contains the parse result and other binding information.</param>
    /// <returns></returns>
    public abstract IBindingResult CreateBindingResult(PropertyBindingInfo bindingInfo);
}