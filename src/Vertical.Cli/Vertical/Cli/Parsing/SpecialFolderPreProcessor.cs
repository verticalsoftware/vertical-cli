using System.Text.RegularExpressions;

namespace Vertical.Cli.Parsing;

internal static partial class SpecialFolderPreProcessor
{
    internal static void Handle(LinkedList<string> argumentList)
    {
        argumentList.EvaluateEach(TryReplaceSpecialFolderSymbol);
    }

    private static string TryReplaceSpecialFolderSymbol(string value)
    {
        return SpecialFolderRegex().Replace(value, match =>
        {
            var constant = match.Groups[1].Value;

            return Enum.TryParse(constant, out Environment.SpecialFolder specialFolder)
                ? Environment.GetFolderPath(specialFolder)
                : match.Value;
        });
    }

    [GeneratedRegex(@"\$\(SpecialFolder.(\w+)\)")]
    private static partial Regex SpecialFolderRegex();
}