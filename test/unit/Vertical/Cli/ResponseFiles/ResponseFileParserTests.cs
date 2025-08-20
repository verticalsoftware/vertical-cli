using System.Text;
using Shouldly;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.ResponseFiles;

public class ResponseFileParserTests
{
    private static readonly Parser _parser = new(new CommandLineOptions());
    
    [Fact]
    public async Task ParseResponseFileTokens_Returns_OriginalList()
    {
        var tokenList = new LinkedList<Token>(_parser.ParseArguments(
        [
            "-a",
            "--option",
            "file.txt",
            "--",
            "password=secret"
        ]));

        var errors = new List<UsageError>();

        var result = await ResponseFileParser.ParseResponseFileTokensAsync(
            _parser,
            tokenList,
            errors,
            _ => null!);
        
        result.ShouldBe(tokenList);
        errors.ShouldBeEmpty();
    }

    [Fact]
    public async Task ParseResponseFileTokens_Returns_ModifiedTokenList()
    {
        var streams = new Dictionary<string, string>()
        {
            ["login-info.rsp"] =
                """
                --user-id root
                --password P@ssw0rd!
                """,
            ["connection-props.rsp"] =
                """
                --connect-timeout:30s
                --useSSL:true --pool
                --buffer-size:10mb
                --prop 'Transaction Mode=Isolated'
                """
        };
        
        var tokenList = new LinkedList<Token>(_parser.ParseArguments(
        [
            "[@login-info.rsp]",
            "[@connection-props.rsp]",
            "--log-verbose",
            "--server:postgres.com",
            "--database:core"
        ]));

        var provider = (string resource) => new MemoryStream(Encoding.UTF8.GetBytes(streams[resource]));
        var errors = new List<UsageError>();

        var result = await ResponseFileParser.ParseResponseFileTokensAsync(
            _parser,
            tokenList,
            errors,
            provider);

        errors.ShouldBeEmpty();
        await Verify(result.Select(token => token.Text));
    }
}
