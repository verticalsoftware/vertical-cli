namespace Vertical.Cli.ScenarioTests.Common;

public sealed class XmlHelpFileAssemblyFixture
{
    public byte[] XmlHelpData { get; } = File.ReadAllBytes("Common/help.xml");

    public byte[] XmlPrefixedHelpData { get; } = File.ReadAllBytes("Common/help_prefixed.xml");
}