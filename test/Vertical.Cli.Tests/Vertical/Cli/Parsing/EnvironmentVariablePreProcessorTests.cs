namespace Vertical.Cli.Parsing;

public class EnvironmentVariablePreProcessorTests
{
    [Fact]
    public void Environment_Variables_Replaced()
    {
        var variables = Environment.GetEnvironmentVariables();
        var keys = variables
            .Keys
            .Cast<string>()
            .Take(5)
            .ToArray();

        var args = keys.Select(MakeOption);
        var list = new LinkedList<string>(args);
        
        EnvironmentVariablePreProcessor.Handle(list);

        var expected = keys.Select(key => MakeExpectedValue(variables[key]!)).ToArray();

        Assert.Equal(expected, list);
    }

    private static string MakeOption(string key)
    {
        return Environment.OSVersion.Platform == PlatformID.Win32NT
            ? $"--option=$env:{key}"
            : $"--option=${key}";
    }

    private static string MakeExpectedValue(object value)
    {
        return $"--option={value}";
    }
}