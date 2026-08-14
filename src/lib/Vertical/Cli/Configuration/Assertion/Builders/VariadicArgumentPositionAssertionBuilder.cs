using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class VariadicArgumentPositionAssertionBuilder : IAssertionBuilder
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
        var arguments = context
            .GetPositionArguments(command)
            .ToArray();

        CliSymbol? variadicSymbol = null;

        foreach (var argument in arguments)
        {
            var isVariadic = argument.Arity.IsVariadic;

            if (isVariadic && variadicSymbol is null)
            {
                variadicSymbol = argument;
                continue;
            }

            if (variadicSymbol is null)
                continue;
            
            context.Assertions.Add(new VariadicArgumentPositionAssertion(command, [variadicSymbol, argument]));
        }
    }
}