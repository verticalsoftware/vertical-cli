using Vertical.Cli.ScenarioTests.Common;

namespace Vertical.Cli.ScenarioTests;

public class HappyPath
{
    private readonly TestApplicationFixture _fixture = new();

    [Fact]
    public Task Invoke_Create__Displays_Verified()
    {
        string[] args =
        [
            "create",
            "./image1.png",
            "./image2.png",
            "--out=./archive.gz",
            "--split-size=10m",
            "--compute-sha",
            "--overwrite",
            "--include-metadata",
            "--secret=~/.ssh/id_rsa.pub",
            "--timeout=00:00:15",
            "--property:tags=images",
            "--property:encoding=.png"
        ];

        return Verify(_fixture.GetOutputAsync(args));
    }

    [Fact]
    public Task Invoke_Extract_Displays_Verified()
    {
        string[] args =
        [
            "extract",
            ".archive.gz",
            "--out=.",
            "--compute-sha",
            "--overwrite",
            "--secret=~/.ssh/id_rsa",
            "--timeout=00:00:15"
        ];

        return Verify(_fixture.GetOutputAsync(args));
    }
}