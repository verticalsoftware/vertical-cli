namespace Vertical.Cli.ScenarioTests.Common;

public sealed class XmlHelpFileAssemblyFixture
{
    public byte[] XmlHelpData { get; } = File.ReadAllBytes("Common/help.xml");
}