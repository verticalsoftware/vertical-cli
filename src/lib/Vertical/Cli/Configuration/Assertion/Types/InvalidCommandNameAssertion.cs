namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Indicates a command name is invalid.
/// </summary>
public sealed class InvalidCommandNameAssertion : ConfigurationAssertion
{
    internal InvalidCommandNameAssertion(Command command)
    {
        Command = command;
    }

    public Command Command { get; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Create(Command);

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return $"Invalid name '{Command.Name}'";
    }
}