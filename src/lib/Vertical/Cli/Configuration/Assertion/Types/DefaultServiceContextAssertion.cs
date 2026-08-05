namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Indicates one or more commands require the application's service provider, but one wasn't
/// configured.
/// </summary>
public sealed class DefaultServiceContextAssertion : ConfigurationAssertion
{
    internal DefaultServiceContextAssertion(Command[] commands)
    {
        Commands = commands;
    }

    /// <summary>
    /// Gets the commands that require services.
    /// </summary>
    public Command[] Commands { get; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Services;

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return "Application has no service provider configured. The following handlers cannot be resolved:";
    }

    /// <inheritdoc />
    public override IEnumerable<string> GetIssueDetail()
    {
        return Commands
            .Select(command => $"'{command.Path}' -> IHandler<{command.ModelType}>");
    }
}