using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class InvalidVariadicArgumentsBuilder : IAssertionBuilder
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
        var arguments = context.GetPositionArguments(command).ToArray();

        if (arguments.Where(symbol => symbol.Arity.IsVariadic).ToArray() is { Length: > 1 } variadicSymbols)
        {
            context.Assertions.Add(new MultipleVariadicArgumentsAssertion(command, variadicSymbols));
            return;
        }

        var variadicArgument = arguments.FirstOrDefault(symbol => symbol.Arity.IsVariadic);
        if (variadicArgument is null ||  arguments.IndexOf(variadicArgument) == arguments.Length - 1)
            return;
        
        context.Assertions.Add(new VariadicArgumentPositionAssertion(command, arguments));
    }
}