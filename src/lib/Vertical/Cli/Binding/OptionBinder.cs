using Vertical.Cli.Configuration;
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
    public ArgumentBinding CreateBinding(IBindingCreateContext bindingCreateContext, SymbolDefinition symbol)
    {
        var arguments = bindingCreateContext
            .SemanticArguments
            .GetOptionArguments(symbol);

        var typedSymbol = (SymbolDefinition<T>)symbol;

        var values = arguments
            .Select(argument => GetValue(bindingCreateContext, typedSymbol, argument))
            .ToArray();

        if (values.Length == 0 && typedSymbol.DefaultProvider != null)
        {
            values = new[] { typedSymbol.DefaultProvider() };
        }
        
        symbol.ValidateArity(values.Length);

        return new ArgumentBinding<T>(symbol, values);
    }
    
    private static T GetValue(
        IBindingCreateContext bindingCreateContext,
        SymbolDefinition<T> symbol,
        SemanticArgument argument)
    {
        try
        {
            switch (argument)
            {
                case { ArgumentSyntax.HasOperand: true }:

                    return bindingCreateContext.GetBindingValue(symbol, argument.ArgumentSyntax.OperandValue);
                
                case { CandidateOperandSyntax: not null } when !bindingCreateContext
                    .SymbolIdentities
                    .Contains(argument.CandidateOperandSyntax.Text):
                    
                    argument.AcceptOperand();

                    return bindingCreateContext.GetBindingValue(symbol, argument.CandidateOperandSyntax.Text);
                
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