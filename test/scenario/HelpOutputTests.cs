using Vertical.Cli.ScenarioTests.Common;

namespace Vertical.Cli.ScenarioTests;

public class HelpOutputTests
{
    private readonly TestApplicationFixture _fixture = new();

    [Fact]
    public Task Invoke_Displays_Help_With_No_SubCommand()
    {
        return Verify(_fixture.GetOutputAsync([]));
    }

    [Fact]
    public Task Invoke_Create_Help_Displays_Article()
    {
        return Verify(_fixture.GetOutputAsync(["create", "-?"]));
    }

    [Fact]
    public Task Invoke_Extract_Help_Displays_Article()
    {
        return Verify(_fixture.GetOutputAsync(["extract", "-?"]));
    }
}