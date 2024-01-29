namespace Vertical.Cli.Utilities;

public class ResponseFileReaderTests
{
    [Fact]
    public void ReturnsSingleToken()
    {
        var tokens = GetTokens("token");
        
        Assert.Single(tokens, value => value == "token");
    }

    [Fact]
    public void ReturnsTokensOnDifferentLines()
    {
        var tokens = GetTokens("red", "green", "blue");
        
        Assert.Collection(tokens,
            value => Assert.Equal("red", value),
            value => Assert.Equal("green", value),
            value => Assert.Equal("blue", value));
    }

    [Fact]
    public void ReturnsTokensOnSameLine()
    {
        var tokens = GetTokens("-a -b -c");
        
        Assert.Collection(tokens,
            value => Assert.Equal("-a", value),
            value => Assert.Equal("-b", value),
            value => Assert.Equal("-c", value));
    }

    [Fact]
    public void IgnoresLeadingWhiteSpace()
    {
        var tokens = GetTokens("  red green");

        Assert.Collection(tokens,
            value => Assert.Equal("red", value),
            value => Assert.Equal("green", value)); 
    }

    [Fact]
    public void IgnoresTrailingWhiteSpace()
    {
        var tokens = GetTokens("red green  ");
        
        Assert.Collection(tokens,
            value => Assert.Equal("red", value),
            value => Assert.Equal("green", value));
    }

    [Fact]
    public void ReturnsSingleQuotedToken()
    {
        var tokens = GetTokens("'red green'");
        
        Assert.Single(tokens, value => value == "red green");
    }

    [Fact]
    public void ReturnsDoubleQuotedToken()
    {
        var tokens = GetTokens("\"red green\"");

        Assert.Single(tokens, value => value == "red green");
    }

    [Fact]
    public void ReturnsQuotedAndUnquotedTokens()
    {
        var tokens = GetTokens("'red' blue");
        
        Assert.Collection(tokens,
            value => Assert.Equal("red", value),
            value => Assert.Equal("blue", value));
    }

    private static IEnumerable<string> GetTokens(params string[] input)
    {
        using var reader = new StringReader(string.Join(Environment.NewLine, input));

        return ResponseFileReader.ReadTokens(reader, "test/response-file.rsp");
    }
}