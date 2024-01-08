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
    public ArgumentBinding CreateBinding(IBindingPath bindingPath, SymbolDefinition symbol)
    {
        var arguments = bindingPath
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

        bool value;

        switch (argument)
        {
            case { ArgumentSyntax.HasOperand: true }:
                
                if (!bool.TryParse(argument.ArgumentSyntax.OperandValue, out value))
                    throw InvocationExceptions.InvalidSwitchArgument(symbol, argument.ArgumentSyntax.OperandValue);

                return value;
            
            // Only consume the argument it is "true" or "false"
            case { CandidateOperandSyntax.Type: SymbolSyntaxType.Simple } when bool.TryParse(
                argument.CandidateOperandSyntax.Text, out value):
                argument.AcceptOperand();
                return value;
            
            default:
                return true;
        }
    }
}