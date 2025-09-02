namespace Vertical.Cli.Help;

public class DefaultHelpProviderTests
{
    [Fact]
    public async Task DefaultHelpProvider_Prints_SubCommands()
    {
        var rootCommand = new RootCommand("test", helpTag: "Verifies the help provider prints the sub command list");
        rootCommand.AddCommand(new Command("info", helpTag: "Print information about the application"));
        rootCommand.AddCommand(new Command("version", helpTag: "Print the engine version"));

        var app = new CommandLineBuilder(rootCommand)
            .ConfigureMiddleware(middleware => middleware.AddHelpOption())
            .Build();

        var console = new StringConsole();

        await app.RunAsync(["--help"], console);
        await Verify(console.ToString());
    }
}