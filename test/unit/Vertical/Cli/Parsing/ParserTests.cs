using Shouldly;

namespace Vertical.Cli.Parsing;

public class ParserTests
{
    private sealed class TokenConverter : WriteOnlyJsonConverter<Token>
    {
        /// <inheritdoc />
        public override void Write(VerifyJsonWriter writer, Token value)
        {
            writer.WriteStartObject();
            writer.WriteProperty("Kind", value.Kind);
            writer.WriteProperty("Text", value.Text);
            writer.WriteProperty("ParsePosition", value.ParsePosition);
            writer.WriteProperty("SyntaxKind", value.Syntax.Kind);
            writer.WriteProperty("Symbol", value.Syntax.Kind == SyntaxKind.PrefixedIdentifier
                ? value.Symbol
                : "(none)");
            writer.WriteProperty("Parameter", value.Syntax.HasParameter
                ? value.ParameterValue
                : "(none)");
            writer.WriteEndObject();
        }
    }
    
    [Fact]
    public Task ParseArguments_Returns_Expected()
    {
        string[] arguments =
        [
            "export",
            "--table:Logs",
            "--server",
            "database.mysql.com",
            "--database",
            "security",
            "--",
            "./var/logs",
            "-verbose-logging:true",
            "/failFast=true"
        ];

        var tokens = new Parser(new CommandLineOptions()).ParseArguments(arguments).ToArray();
        var settings = new VerifySettings();
        settings.AddExtraSettings(serializer => serializer.Converters.Add(new TokenConverter()));

        return Verify(tokens, settings: settings);
    }

    [Fact]
    public Task ParseArguments_Identifies_Leading_Directives()
    {
        string[] arguments =
        [
            "[debug]",
            "[diagram]",
            "command",
            "[option]"
        ];

        var tokens = new Parser(new CommandLineOptions()).ParseArguments(arguments).ToArray();
        var settings = new VerifySettings();
        settings.AddExtraSettings(serializer => serializer.Converters.Add(new TokenConverter()));

        return Verify(tokens, settings: settings);
    }

    [Fact]
    public Task CreateSemanticTokens_Returns_Expected()
    {
        string[] arguments =
        [
            "-a",
            "-abc",
            "-abc:parameter",
            "--option",
            "/option",
            "argument",
            "--",
            "terminated"
        ];

        var parser = new Parser(new CommandLineOptions());
        var tokens = parser.ParseArguments(arguments).ToArray();
        var semanticTokens = tokens.SelectMany(token => parser.CreateSemanticTokens(token)).ToArray();
        var settings = new VerifySettings();
        settings.AddExtraSettings(serializer => serializer.Converters.Add(new TokenConverter()));

        return Verify(semanticTokens, settings: settings);
    }
    
    
}