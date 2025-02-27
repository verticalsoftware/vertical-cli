using Shouldly;

namespace Vertical.Cli.Utilities;

public class ArgumentsTest
{
    [Fact]
    public void Split_Returns_Expected()
    {
        const string input =
            """
            # this is a comment, should be ignored
            --server:localhost
            -u testy -p P@ssw0rd!
            --key "has space"
            """;

        Arguments
            .ReadAll(new StringReader(input))
            .ToArray()
            .ShouldBe(
                [
                    "--server:localhost",
                    "-u",
                    "testy",
                    "-p",
                    "P@ssw0rd!",
                    "--key",
                    "has space"
                ]);
    }
}