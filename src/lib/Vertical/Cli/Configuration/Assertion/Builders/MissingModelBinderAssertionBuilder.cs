using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class MissingModelBinderAssertionBuilder : IAssertionBuilder
{
    /// <inheritdoc />
    public void Build(AssertionContext context)
    {
        foreach (var command in context.Commands)
        {
            ValidateModelBinder(context, command);
        }
    }

    private static void ValidateModelBinder(AssertionContext context, Command command)
    {
        if (!command.CanCreateCallSite)
            return;

        if (context.GetModelConfiguration(command!) is { HasModelBinder: true })
            return;
        
        context.Assertions.Add(new MissingModelBinderAssertion(command.ModelType!));
    }
}