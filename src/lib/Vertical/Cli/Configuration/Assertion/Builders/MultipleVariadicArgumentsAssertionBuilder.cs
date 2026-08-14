using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class MultipleVariadicArgumentsAssertionBuilder : IAssertionBuilder
{
    /// <inheritdoc />
    public void Build(AssertionContext context)
    {
        foreach (var command in context.CallSites)
        {
            ValidateVariadicArgumentOrder(context, command);
        }
    }

    private static void ValidateVariadicArgumentOrder(AssertionContext context, Command command)
    {
        var arguments = context.GetPositionArguments(command);
        var variadicArguments = arguments
            .Where(symbol => symbol.Arity.IsVariadic)
            .Cast<ICliSymbol>()
            .ToArray();

        if (variadicArguments.Length <= 1)
            return;
        
        context.Assertions.Add(new MultipleVariadicArgumentsAssertion(command, variadicArguments));
    }
}