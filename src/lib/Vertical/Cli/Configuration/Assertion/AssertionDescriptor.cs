using Vertical.Cli.Binding;

namespace Vertical.Cli.Configuration.Assertion;

/// <summary>
/// Defines descriptors for symbols.
/// </summary>
public static class AssertionDescriptor
{
    /// <summary>
    /// Creates a symbol descriptor. 
    /// </summary>
    /// <param name="symbol">The symbol reference.</param>
    /// <returns><see cref="string"/></returns>
    public static string Create(object symbol)
    {
        return symbol switch
        {
            CliSymbol { Kind: SymbolKind.Option or SymbolKind.Switch } bound => 
                $"<{bound.ModelType.Name}, {bound.ValueType.Name}:{bound.BindingName}> {bound.Kind} {string.Join(", ", bound.Aliases)}",
            
            CliSymbol { Kind: SymbolKind.PositionArgument } bound =>
                $"<{bound.ModelType.Name}, {bound.ValueType.Name}:{bound.BindingName}> {bound.Kind} @{bound.OrdinalPosition}",
            
            ICliSymbol { Kind: SymbolKind.Directive } directive => 
                $"Directive {string.Join(", ", directive.Aliases)}",
            
            ICliSymbol { Kind: SymbolKind.Switch } mwSwitch => $"Middleware switch {string.Join(", ", mwSwitch.Aliases)}",
            
            IBindingSource binding => $"({symbol.GetType().Name} <{binding.ModelType.Name}, {binding.ValueType.Name}:{binding.BindingName}>)",
            
            _ => throw new NotSupportedException($"{symbol.GetType()}")
        };
    }
}