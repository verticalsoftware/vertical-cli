using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Binds semantic arguments to options.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
internal sealed class OptionBinder<T> : IBinder
{
    internal static OptionBinder<T> Instance { get; } = new();

    /// <inheritdoc />
    public ArgumentBinding CreateBinding(IBindingContext bindingContext, SymbolDefinition symbol)
    {
        var arguments = bindingContext
            .SemanticArguments
            .GetOptionArguments(symbol);

        var typedSymbol = (SymbolDefinition<T>)symbol;

        var values = arguments
            .Select(argument => GetValue(bindingContext, typedSymbol, argument))
            .ToArray();

        if (values.Length == 0 && typedSymbol.DefaultProvider != null)
        {
            values = new[] { typedSymbol.DefaultProvider() };
        }
        
        symbol.ValidateArity(values.Length);

        return new ArgumentBinding<T>(symbol, values);
    }
    
    private static T GetValue(
        IBindingContext bindingContext,
        SymbolDefinition<T> symbol,
        SemanticArgument argument)
    {
        argument.Accept();
        
        try
        {
            switch (argument)
            {
                case { HasOperand: true }:
                    argument.AcceptOperand();
                    return bindingContext.GetBindingValue(symbol, argument.OperandValue);
                
                default:
                    throw InvocationExceptions.OptionMissingOperand(symbol);
            }
        }
        finally
        {
            argument.Accept();
        }
    }
}