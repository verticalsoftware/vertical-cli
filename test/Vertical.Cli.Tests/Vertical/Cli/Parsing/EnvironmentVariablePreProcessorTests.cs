namespace Vertical.Cli.Parsing;

public class EnvironmentVariablePreProcessorTests
{
    [Fact]
    public void Environment_Variables_Replaced()
    {
        Environment.SetEnvironmentVariable("VERTICAL_CLI_ARG_A", "value-a");
        Environment.SetEnvironmentVariable("VERTICAL_CLI_ARG_B", "value-b");
        
        string[] args = ["--option=$VERTICAL_CLI_ARG_A", "--option=$VERTICAL_CLI_ARG_B"];
        var list = new LinkedList<string>(args);
        
        EnvironmentVariablePreProcessor.Handle(list);

        string[] expected = ["--option=value-a", "--option=value-b"];

        Assert.Equal(expected, list);
    }
}