namespace Vertical.Cli.Configuration;

internal sealed class ActionTaskConfiguration : ModelessTaskConfiguration
{
    private readonly Func<CliCommand, CliOptions, Task<int>> _handler;

    internal ActionTaskConfiguration(
        CliCommand command,
        int index,
        string[] names,
        string? description,
        CliScope scope,
        Func<CliCommand, CliOptions, Task<int>> handler)
        : base(command, index, names, description, optionGroup: null, scope)
    {
        _handler = handler;
    }

    public override Task<int> InvokeAsync(CliCommand command, CliOptions options)
    {
        return _handler(command, options);
    }
}