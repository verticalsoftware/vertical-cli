namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Indicates a command is a dead end, e.g. has no handler and no sub commands.
/// </summary>
public class DeadEndCommandAssertion : ConfigurationAssertion
{
    /// <inheritdoc />
    internal DeadEndCommandAssertion(Command command)
    {
        Command = command;
    }

    /// <summary>
    /// Gets the dead-end command.
    /// </summary>
    public Command Command { get; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Create(Command);

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return "No handler set/no sub-commands are configured (path is a dead-end).";
    }
}