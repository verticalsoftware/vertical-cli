namespace Vertical.Cli.Configuration;

internal class HelpTaskConfiguration : ModelessTaskConfiguration
{
    private readonly int _result;

    internal HelpTaskConfiguration(
        CliCommand command,
        int index,
        string[] names, 
        string? description,
        string? optionGroup,
        CliScope scope,
        int result) 
        : base(command, index, names, description, optionGroup, scope)
    {
        _result = result;
    }

    public override Task<int> InvokeAsync(CliCommand command, CliOptions options)
    {
        WriteHelpToConsole(command, options);
        return Task.FromResult(_result);
    }

    internal static void WriteHelpToConsole(CliCommand command, CliOptions options)
    {
        var provider = options.HelpProvider;
        var content = provider.GetContent(command);
        
        Console.Write(content);
    }
}