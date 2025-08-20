using Argon;
using Shouldly;

namespace Vertical.Cli.Parsing;

public class TokenSyntaxTests
{
    private sealed class TokenSyntaxConverter : WriteOnlyJsonConverter<TokenSyntax>
    {
        /// <inheritdoc />
        public override void Write(VerifyJsonWriter writer, TokenSyntax value)
        {
            writer.WriteStartObject();
            writer
                .WriteProperty("Text", value.Text)
                .WriteProperty("Kind", value.Kind)
                .WriteProperty("EnclosedSpan", value.EnclosedSpan)
                .WriteProperty("SymbolConvention", value.SymbolConvention)
                .WriteProperty("IdentifierSpan", value.SymbolSpan)
                .WriteProperty("IdentifierPrefixSpan", value.SymbolPrefixSpan)
                .WriteProperty("NonPrefixedIdentifierSpan", value.NonPrefixedSymbolSpan)
                .WriteProperty("ParameterSyntaxSpan", value.ParameterSyntaxSpan)
                .WriteProperty("ParameterAssignmentOperator", value.ParameterAssignmentOperator)
                .WriteProperty("ParameterSpan", value.ParameterSpan);
            writer.WriteEndObject();
        }
    }
    
    [Fact]
    public Task TokenSyntax_Returns_Expected()
    {
        string[] args =
        [
            "non-decorated",
            "-a",
            "-a:param",
            "-a=param",
            "-abc",
            "-abc:param",
            "-abc=param",
            "--option",
            "--option=param",
            "--option:param",
            "/option",
            "/option=param",
            "/option:param",
            "[directive]",
            "--"
        ];

        var fact = args.Select(TokenSyntax.Parse).ToArray();
        var settings = new VerifySettings();
        settings.AddExtraSettings(serializer =>
        {
            serializer.Converters.Add(new TokenSyntaxConverter());
        });
        
        return Verify(fact, settings: settings);
    }

    [Fact]
    public void ParseDirective_Returns_Identifier()
    {
        DirectiveSyntax.Parse("[log-level]").IdentifierSpan.ToString().ShouldBe("log-level");
    }
    
    [Fact]
    public void ParseDirective_Returns_Identifier_With_Parameter()
    {
        DirectiveSyntax.Parse("[log-level:debug]").IdentifierSpan.ToString().ShouldBe("log-level");
        DirectiveSyntax.Parse("[log-level=debug]").IdentifierSpan.ToString().ShouldBe("log-level");
    }

    [Fact]
    public void ParseDirective_Returns_Parameter()
    {
        DirectiveSyntax.Parse("[log-level:debug]").ParameterSpan.ToString().ShouldBe("debug");
        DirectiveSyntax.Parse("[log-level=debug]").ParameterSpan.ToString().ShouldBe("debug");
    }
}