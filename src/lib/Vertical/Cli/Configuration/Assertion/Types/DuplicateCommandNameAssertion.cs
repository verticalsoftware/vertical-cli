namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Indicates a command has two or more sub commands that share the same name.
/// </summary>
public sealed class DuplicateCommandNameAssertion : ConfigurationAssertion
{
    /// <inheritdoc />
    internal DuplicateCommandNameAssertion(Command command, string name, IEnumerable<Command> subCommands)
    {
        Command = command;
        Name = name;
        SubCommands = subCommands;
    }

    public Command Command { get; }
    public string Name { get; }
    public IEnumerable<Command> SubCommands { get; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Create(Command);

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return $"Sub command '{Name}' used across multiple sub command:";
    }

    /// <inheritdoc />
    public override IEnumerable<string> GetIssueDetail()
    {
        return SubCommands
            .Select(command => $"CallSite: {command.ModelType?.ToString() ?? "(abstract)"}, sub commands: {command.SubCommands.Count}");
    }
}