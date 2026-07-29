using Vertical.Cli.Parsing;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Indicates an invalid response argument.
/// </summary>
public sealed class ResponseArgumentNotSupportedError : CommandLineError
{
    /// <inheritdoc />
    public ResponseArgumentNotSupportedError(string annotation, int lineNumber, SyntaxKind syntaxKind, string argument)
        : base(FormatMessage(annotation, lineNumber, syntaxKind, argument))
    {
        Annotation = annotation;
        LineNumber = lineNumber;
        SyntaxKind = syntaxKind;
        Argument = argument;
    }

    public string Annotation { get; }

    public int LineNumber { get; }

    public SyntaxKind SyntaxKind { get; }

    public string Argument { get; }

    private static string FormatMessage(string annotation, int lineNumber, SyntaxKind syntaxKind, string argument)
    {
        var description = syntaxKind switch
        {
            SyntaxKind.Annotation => "Annotation",
            SyntaxKind.OptionsTerminator => "Options terminating token",
            SyntaxKind.Directive => "Directive",
            _ => throw new NotSupportedException($"{syntaxKind}")
        };

        return $"{description} in response resource {annotation} on line {lineNumber} not supported: '{argument}'";
    }
}