using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class DeadEndCommandsBuilder : IAssertionBuilder
{
    /// <inheritdoc />
    public void Build(AssertionContext context)
    {
        foreach (var command in context.Commands)
        {
            ValidateCallSite(context, command);
        }
    }

    private static void ValidateCallSite(AssertionContext context, Command command)
    {
        if (command.CanCreateCallSite || command.SubCommands.Count > 0)
            return;
        
        context.Assertions.Add(new DeadEndCommandAssertion(command));               
    }
}