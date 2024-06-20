namespace Vertical.Cli.Configuration;

internal class HelpTaskConfiguration : ModelessTaskConfiguration
{
    private readonly int _result;

    internal HelpTaskConfiguration(
        string[] names, 
        string? description, 
        CliScope scope,
        int result) 
        : base(names, description, scope)
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