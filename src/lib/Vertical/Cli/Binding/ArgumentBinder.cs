using Vertical.Cli.Configuration;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Binds semantic arguments to argument symbols.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
internal sealed class ArgumentBinder<T> : IBinder where T : notnull
{
    internal static ArgumentBinder<T> Instance { get; } = new();

    /// <inheritdoc />
    public ArgumentBinding CreateBinding(IBindingPath bindingPath, SymbolDefinition symbol)
    {
        var arguments = bindingPath
            .SemanticArguments
            .GetValueArguments(symbol);

        var typedSymbol = (SymbolDefinition<T>)symbol;

        var values = arguments
            .Select(argument =>
            {
                argument.Accept();
                return bindingPath.GetBindingValue(typedSymbol, argument.ArgumentSyntax.Text);
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