using Vertical.Cli.Help;
using Vertical.Cli.ScenarioTests.Common;

namespace Vertical.Cli.ScenarioTests;

public class XmlHelpOutputTests
{
    private readonly XmlHelpFileAssemblyFixture _xmlDataFixture;
    private readonly TestApplicationFixtureNoCodeHelp _fixture = new();

    public XmlHelpOutputTests(XmlHelpFileAssemblyFixture xmlDataFixture)
    {
        _xmlDataFixture = xmlDataFixture;
    }

    [Theory]
   [InlineData(false), InlineData(true)]
    public Task Invoke_Displays_Help_With_No_SubCommand(bool prefixed)
    {
        ConfigureXmlProvider(prefixed);
        return Verify(_fixture.GetOutputAsync([]));
    }

    [Theory]
   [InlineData(false), InlineData(true)]
    public Task Invoke_Create_Help_Displays_Article(bool prefixed)
    {
        ConfigureXmlProvider(prefixed);
        return Verify(_fixture.GetOutputAsync(["create", "-?"]));
    }

    [Theory]
   [InlineData(false), InlineData(true)]
    public Task Invoke_Extract_Help_Displays_Article(bool prefixed)
    {
        ConfigureXmlProvider(prefixed);
        return Verify(_fixture.GetOutputAsync(["extract", "-?"]));
    }

    private void ConfigureXmlProvider(bool prefixed)
    {
        var helpData = prefixed ? _xmlDataFixture.XmlPrefixedHelpData : _xmlDataFixture.XmlHelpData;
        
        _fixture.Application.ConfigureHelp(options => options.HelpProvider = new XmlHelpProvider(
            () => new MemoryStream(helpData)));
    }
}