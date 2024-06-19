namespace Vertical.Cli.Configuration;

internal sealed class ActionTaskConfiguration : ModelessTaskConfiguration
{
    private readonly Func<CliCommand, CliOptions, Task<int>> _handler;

    internal ActionTaskConfiguration(
        string[] names,
        string? description,
        CliScope scope,
        Func<CliCommand, CliOptions, Task<int>> handler)
        : base(names, description, scope)
    {
        _handler = handler;
    }

    public override Task<int> InvokeAsync(CliCommand command, CliOptions options)
    {
        return _handler(command, options);
    }
}