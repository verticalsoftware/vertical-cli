using System.Text.RegularExpressions;
using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Parsing;

public partial class ArgumentSyntax
{
    /// <summary>
    /// Parses the syntax of an argument.
    /// </summary>
    /// <param name="argument">The argument value to parse.</param>
    /// <returns>A <see cref="ArgumentSyntax"/></returns>
    public static ArgumentSyntax Parse(string argument)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(argument);

        return argument switch
        {
            "--" => new ArgumentSyntax(argument, SyntaxKind.OptionsTerminator),
            ['-', '-', ..] => ParseGnuOption(argument),
            ['-', _] => ParsePosixOption(argument),
            ['-', ..] => ParsePosixGroup(argument),
            ['[', .., ']'] => ParseDirective(argument),
            ['@', ..] => new ArgumentSyntax(argument, SyntaxKind.Annotation, parameterValue: argument[1..]),
            _ => new ArgumentSyntax(argument, SyntaxKind.None)
        };
    }

    /// <summary>
    /// Gets the probable syntax kind for the given argument.
    /// </summary>
    /// <param name="argument">The argument ot parse.</param>
    /// <returns><see cref="SyntaxKind"/></returns>
    /// <remarks>
    /// This method matches the basic structure of arguments and returns the probably kind of
    /// syntax it is.
    /// </remarks>
    public static SyntaxKind GetSyntaxKind(string argument)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(argument);

        return argument switch
        {
            "--" => SyntaxKind.OptionsTerminator,
            ['-', '-', ..] => SyntaxKind.Option,
            ['-', _] => SyntaxKind.Option,
            ['-', ..] => SyntaxKind.OptionGroup,
            ['[', .., ']'] => SyntaxKind.Directive,
            ['@', ..] => SyntaxKind.Annotation,
            _ => SyntaxKind.None
        };
    }

    private static ArgumentSyntax ParseDirective(string argument)
    {
        return DirectivePattern().Match(argument) is
            { Success: true } match
            ? new ArgumentSyntax(
                argument,
                SyntaxKind.Directive,
                GetId(match),
                GetSeparatorChar(match),
                GetParameter(match))
            : new ArgumentSyntax(argument, SyntaxKind.None);
    }

    private static ArgumentSyntax ParsePosixGroup(string argument)
    {
        return PosixGroupPattern().Match(argument) is { Success: true } match
            ? new ArgumentSyntax(
                argument,
                SyntaxKind.OptionGroup,
                GetId(match),
                GetSeparatorChar(match),
                GetParameter(match))
            : new ArgumentSyntax(argument, SyntaxKind.None);
    }

    private static ArgumentSyntax ParsePosixOption(string argument)
    {
        return PosixOptionPattern().Match(argument) is { Success: true } match
            ? new ArgumentSyntax(
                argument,
                SyntaxKind.Option,
                GetId(match),
                GetSeparatorChar(match),
                GetParameter(match))
            : new ArgumentSyntax(argument, SyntaxKind.None);
    }

    private static ArgumentSyntax ParseGnuOption(string argument)
    {
        return GnuPattern().Match(argument) is
            { Success: true } match
            ? new ArgumentSyntax(
                argument, 
                SyntaxKind.Option, 
                GetId(match), 
                GetSeparatorChar(match),
                GetParameter(match))
            : new ArgumentSyntax(argument, SyntaxKind.None);
    }

    private static string GetId(Match match) => match.Groups["id"].Value;

    private static char? GetSeparatorChar(Match match)
    {
        return match.Groups["sep"].Value is { Length: > 0 } token
            ? token[0]
            : null;
    }

    private static string? GetParameter(Match match)
    {
        return match.Groups["param"].Value is { Length: > 0 } param
            ? param
            : null;
    }

    [GeneratedRegex("^(?<id>--[a-zA-Z0-9](?:[a-zA-Z0-9-]+)?)((?<sep>[:=])(?<param>.+))?$")]
    private static partial Regex GnuPattern();
    
    [GeneratedRegex("^(?<id>-[a-zA-Z0-9]+)((?<sep>[:=])(?<param>.+))?$")]
    private static partial Regex PosixGroupPattern();
    
    [GeneratedRegex("^(?<id>-[a-zA-Z0-9])((?<sep>[:=])(?<param>.+))?$")]
    private static partial Regex PosixOptionPattern();
    
    [GeneratedRegex(@"^\[(?<id>[a-zA-Z0-9-]+)((?<sep>[:=])(?<param>.+))?\]$")]
    private static partial Regex DirectivePattern();
}