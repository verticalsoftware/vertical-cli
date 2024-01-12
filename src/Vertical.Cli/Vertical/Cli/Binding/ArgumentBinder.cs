using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Binds semantic arguments to argument symbols.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
internal sealed class ArgumentBinder<T> : IBinder
{
    internal static ArgumentBinder<T> Instance { get; } = new();

    /// <inheritdoc />
    public ArgumentBinding CreateBinding(IBindingContext bindingContext, SymbolDefinition symbol)
    {
        var arguments = bindingContext
            .SemanticArguments
            .GetValueArguments(symbol);

        var typedSymbol = (SymbolDefinition<T>)symbol;

        var values = arguments
            .Select(argument =>
            {
                argument.Accept();
                
                return bindingContext.GetBindingValue(
                    typedSymbol, 
                    argument.ArgumentSyntax.Text);
            })
            .ToArray();

        if (values.Length == 0 && typedSymbol.DefaultProvider != null)
        {
            values = new[] { typedSymbol.DefaultProvider() };
        }
        
        symbol.ValidateArity(values.Length);

        return new ArgumentBinding<T>(symbol, values);
    }
}