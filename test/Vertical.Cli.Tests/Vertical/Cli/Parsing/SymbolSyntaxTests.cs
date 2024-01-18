namespace Vertical.Cli.Parsing;

public class SymbolSyntaxTests
{
    [Theory, ClassData(typeof(ParseTheoryData))]
    public void ParseCreatesExpectedSyntax(string input,
        SymbolSyntaxType expectedType,
        string expectedText,
        string[] expectedIdentifiers,
        string expectedOperand,
        string expectedOperandExpr)
    {
        var syntax = SymbolSyntax.Parse(input);
        Assert.Equal(expectedType, syntax.Type);
        Assert.Equal(expectedText, syntax.Text);
        Assert.Equal(expectedIdentifiers, syntax.Identifiers);
        Assert.Equal(expectedOperand, syntax.OperandValue);
        Assert.Equal(expectedOperandExpr, syntax.OperandExpression);
        Assert.Equal(expectedIdentifiers.Length > 0, syntax.HasIdentifiers); 
        Assert.Equal(expectedIdentifiers.Length == 1, syntax.HasSingleIdentifier);
        Assert.Equal(SymbolSyntax.Parse(input).GetHashCode(), syntax.GetHashCode());
        Assert.True(SymbolSyntax.Parse(input) == syntax);
        Assert.False(SymbolSyntax.Parse(input) != syntax);
    }

    public class ParseTheoryData : TheoryData<string, SymbolSyntaxType, string, string[], string, string>
    {
        public ParseTheoryData()
        {
            Add("cmd", SymbolSyntaxType.Simple, "cmd", new[]{"cmd"}, "", "");
            Add("-a", SymbolSyntaxType.PosixPrefixed, "-a", new[]{"-a"}, "", "");
            Add("-abc", SymbolSyntaxType.PosixPrefixed, "-abc", new[]{"-a","-b","-c"}, "", "");
            Add("--long-option", SymbolSyntaxType.GnuPrefixed, "--long-option", new[]{"--long-option"}, "", "");
            Add("/long-option", SymbolSyntaxType.SlashPrefixed, "/long-option", new[]{"/long-option"}, "", "");
            Add("-a=op", SymbolSyntaxType.PosixPrefixed, "-a=op", new[]{"-a"}, "op", "=op");
            Add("--long=op", SymbolSyntaxType.GnuPrefixed, "--long=op", new[]{"--long"}, "op", "=op");
            Add("-ab=op", SymbolSyntaxType.PosixPrefixed, "-ab=op", new[]{"-a","-b"}, "op", "=op");
            Add("/p=op", SymbolSyntaxType.SlashPrefixed, "/p=op", new[]{"/p"}, "op", "=op");
            Add("*non", SymbolSyntaxType.NonIdentifier, "*non", Array.Empty<string>(), "", "");
            Add("--", SymbolSyntaxType.ArgumentTerminator, "--", Array.Empty<string>(), "", "");
        }
    }
}