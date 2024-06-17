using System.Diagnostics.CodeAnalysis;
using Shouldly;

namespace Vertical.Cli.Parsing;

[SuppressMessage("Usage", "xUnit1044:Avoid using TheoryData type arguments that are not serializable")]
public class ArgumentSyntaxTests
{
    [Theory, MemberData(nameof(Theories))]
    public void Parse_Returns_Expected_Structure(string arg, ArgumentSyntax expected)
    {
        var result = ArgumentSyntax.Parse(arg);
        result.ShouldBe(expected);
    }

    [Fact]
    public void Preserves_Arguments_After_Termination()
    {
        var unit = ArgumentParser.Parse(["-a", "-b", "--", "-c", "--long"]);
        unit
            .Select(syntax => syntax.PrefixType)
            .ShouldBe([
                OptionPrefixType.PosixOption,
                OptionPrefixType.PosixOption,
                OptionPrefixType.None,
                OptionPrefixType.None
            ]);
    }

    public static TheoryData<string, ArgumentSyntax> Theories => new TheoryData<string, ArgumentSyntax>
    {
        { "plain", new ArgumentSyntax(OptionPrefixType.None, "plain") },
        // Posix cases
        { "-a", new ArgumentSyntax(OptionPrefixType.PosixOption, "-a", "-a", "a" )},
        { "-a=true", new ArgumentSyntax(OptionPrefixType.PosixOption, "-a=true", "-a", "a", "=", "true" )},
        { "-a:true", new ArgumentSyntax(OptionPrefixType.PosixOption, "-a:true", "-a", "a", ":", "true" )},
        { "-abc", new ArgumentSyntax(OptionPrefixType.PosixOption, "-abc", "-abc", "abc" )},
        { "-abc=true", new ArgumentSyntax(OptionPrefixType.PosixOption, "-abc=true", "-abc", "abc", "=", "true" )},
        { "-abc:true", new ArgumentSyntax(OptionPrefixType.PosixOption, "-abc:true", "-abc", "abc", ":", "true" )},
        // GNU cases
        { "--long", new ArgumentSyntax(OptionPrefixType.GnuOption, "--long", "--long", "long" )},
        { "--long=true", new ArgumentSyntax(OptionPrefixType.GnuOption, "--long=true", "--long", "long", "=", "true" )},
        { "--long:true", new ArgumentSyntax(OptionPrefixType.GnuOption, "--long:true", "--long", "long", ":", "true" )},
        { "--long-option", new ArgumentSyntax(OptionPrefixType.GnuOption, "--long-option", "--long-option", "long-option" )},
        { "--long-option=true", new ArgumentSyntax(OptionPrefixType.GnuOption, "--long-option=true", "--long-option", "long-option", "=", "true" )},
        { "--long-option:true", new ArgumentSyntax(OptionPrefixType.GnuOption, "--long-option:true", "--long-option", "long-option", ":", "true" )}
    };
}