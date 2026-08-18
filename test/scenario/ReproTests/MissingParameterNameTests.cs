using System.Text;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.ScenarioTests.Common;

namespace Vertical.Cli.ScenarioTests.ReproTests;

public class MissingParameterNameTests
{
    public record Model(int? Port, string Path);
    
    [Fact]
    public async Task Run_With_Help_Args_Displays_Default_Option_Parameter_Name()
    {
        var command = new RootCommand("test");
        var console = new TestConsole();
        command.SetHandler<Model>((_,_) => Task.FromResult(0));

        var app = new CommandLineApplication(command);
        app.ConfigureHelp(help => help.HelpProvider = new XmlHelpProvider(
            () => new MemoryStream(Encoding.UTF8.GetBytes(XmlHelp))));
        app.ConfigureParser<Model>(m => m
            .ParseOption(x => x.Port)
            .ParseArgument(x => x.Path, ordinalPosition: 0));
        app.ConfigureModel<Model>(m => m.SetBinder(_ => new Model(0, "c:/")));
        app.UseConsole(console);
        app.AddArgumentConverter(Converters.NullParsable<int>())
            .AddArgumentConverter(Converters.Default);

        await app.RunAsync(["-?"]);
        await Verify(console.ToString());
    }

    private const string XmlHelp =
        """
        <?xml version="1.0" encoding="utf-8" ?>
        <help>
            <topic type="command" id="test">
                <remarks>This is a test command.</remarks>
            </topic>
            <topic type="symbol" id="Vertical.Cli.ScenarioTests.ReproTests.MissingParameterNameTests+Model.Port">
                I have no interesting remarks either...
            </topic>
            <topic type="symbol" id="Vertical.Cli.ScenarioTests.ReproTests.MissingParameterNameTests+Model.Path">
                I have no interesting remarks...
            </topic>
        </help>
        """;
}