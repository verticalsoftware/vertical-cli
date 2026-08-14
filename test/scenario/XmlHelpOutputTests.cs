using Vertical.Cli.Help;
using Vertical.Cli.ScenarioTests.Common;

namespace Vertical.Cli.ScenarioTests;

public class XmlHelpOutputTests
{
    private readonly XmlHelpFileAssemblyFixture _xmlDataFixture;
    private readonly TestApplicationFixture _fixture = new();

    public XmlHelpOutputTests(XmlHelpFileAssemblyFixture xmlDataFixture)
    {
        _xmlDataFixture = xmlDataFixture;
    }

    [Fact]
    public Task Invoke_Displays_Help_With_No_SubCommand()
    {
        ConfigureXmlProvider();
        return Verify(_fixture.GetOutputAsync([]));
    }

    [Fact]
    public Task Invoke_Create_Help_Displays_Article()
    {
        ConfigureXmlProvider();
        return Verify(_fixture.GetOutputAsync(["create", "-?"]));
    }

    [Fact]
    public Task Invoke_Extract_Help_Displays_Article()
    {
        ConfigureXmlProvider();
        return Verify(_fixture.GetOutputAsync(["extract", "-?"]));
    }

    private void ConfigureXmlProvider()
    {
        _fixture.Application.ConfigureHelp(options => options.HelpProvider = 
            new XmlHelpProvider(() => new MemoryStream(_xmlDataFixture.XmlHelpData)));
    }
}