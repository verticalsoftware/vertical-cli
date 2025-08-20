using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vertical.Cli.SourceGenerator;

public static class SyntaxNodeExtensions
{
    public static string? GetSimpleName(this NameSyntax syntax)
    {
        return syntax switch
        {
            SimpleNameSyntax simple => simple.Identifier.Text,
            QualifiedNameSyntax qualified => qualified.Right.GetSimpleName(),
            _ => null
        };
    }
}