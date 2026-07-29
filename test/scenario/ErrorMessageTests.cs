using Vertical.Cli.ScenarioTests.Common;

namespace Vertical.Cli.ScenarioTests;

public class ErrorMessageTests
{
    private readonly TestApplicationFixture _fixture = new();

    [Fact]
    public Task Invoke_With_Missing_Argument_Displays_Error()
    {
        return Verify(_fixture.GetOutputAsync(["create"]));
    }

    [Fact]
    public Task Invoke_With_Missing_Option_Parameter_Displays_Error()
    {
        string[] args =
        [
            "extract",
            ".archive.gz",
            "--out=.",
            "--compute-sha",
            "--overwrite",
            "--secret",
            "--timeout=00:00:15"
        ];

        return Verify(_fixture.GetOutputAsync(args));
    }

    [Fact]
    public Task Invoke_With_Min_Arity_Not_Met_Displays_Error()
    {
        string[] args =
        [
            "extract",
            ".archive.gz",
            "--out=.",
            "--compute-sha",
            "--overwrite",
            "--timeout=00:00:15"
        ];

        return Verify(_fixture.GetOutputAsync(args));
    }

    [Fact]
    public Task Invoke_With_Max_Exceeded_Arity_Displays_Error()
    {
        string[] args =
        [
            "extract",
            ".archive.gz",
            "--out=.",
            "--out=./archives",
            "--compute-sha",
            "--overwrite",
            "--secret=~/.ssh/id_rsa",
            "--timeout=00:00:15"
        ];
        
        return Verify(_fixture.GetOutputAsync(args));
    }

    [Fact]
    public Task Invoke_With_Validation_Issue_Displays_Error()
    {
        string[] args =
        [
            "extract",
            ".archive.gz",
            "--out=./archives",
            "--compute-sha",
            "--overwrite",
            "--secret=~/.ssh/id_rsa",
            "--timeout=00:10:00"
        ];
        
        return Verify(_fixture.GetOutputAsync(args));
    }
}