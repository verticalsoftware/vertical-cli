using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Binds semantic arguments to switch symbols.
/// </summary>
internal sealed class SwitchBinder : IBinder 
{
    internal static SwitchBinder Instance { get; } = new();

    /// <inheritdoc />
    public ArgumentBinding CreateBinding(IBindingContext bindingContext, SymbolDefinition symbol)
    {
        var arguments = bindingContext
            .SemanticArguments
            .GetOptionArguments(symbol);

        var values = arguments
            .Select(argument => GetSwitchValue(symbol, argument))
            .Distinct()
            .ToArray();
        
        var bindingValue = values.Length switch
        {
            0 => symbol.GetValueOrDefault<bool>(),
            1 => values[0],
            _ => throw InvocationExceptions.MaximumArityExceeded(symbol, values.Length)
        };

        return new ArgumentBinding<bool>(symbol, new[] { bindingValue });
    }
    
    private static bool GetSwitchValue(SymbolDefinition symbol, SemanticArgument argument)
    {
        argument.Accept();

        switch (argument)
        {
            case { HasOperand: true } when bool.TryParse(argument.OperandValue, out var value):
                argument.AcceptOperand();
                return value;
            
            case { ArgumentSyntax.HasOperand: true }:
                throw InvocationExceptions.InvalidSwitchArgument(symbol, argument.OperandValue);
            
            default:
                return true;
        }
    }
}