using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion.Builders;

internal sealed class UniqueCommandNamesBuilder : IAssertionBuilder
{
    /// <inheritdoc />
    public void Build(AssertionContext context)
    {
        foreach (var command in context.Commands)
        {
            CheckSubCommandNames(context, command);
        }
    }

    private static void CheckSubCommandNames(AssertionContext context, Command command)
    {
        var duplicateSubCommands = command
            .SubCommands
            .GroupBy(subCommand => subCommand.Name)
            .Where(group => group.Count() > 1);

        foreach (var group in duplicateSubCommands)
        {
            context.Assertions.Add(new DuplicateCommandNameAssertion(command, group.Key, group));
        }
    }
}