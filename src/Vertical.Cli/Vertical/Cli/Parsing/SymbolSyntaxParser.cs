using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Responsible for parsing symbol syntax.
/// </summary>
public static class SymbolSyntaxParser
{
    /// <summary>
    /// Parses a command line argument into any component parts.
    /// </summary>
    /// <param name="str">The string argument.</param>
    /// <returns><see cref="SymbolSyntax"/></returns>
    internal static SymbolSyntax Parse(string str)
    {
        Guard.IsNotNullOrWhiteSpace(str);

        char c;
        var ptr = 0;

        for (; ptr < 2 && ptr < str.Length; ptr++)
        {
            c = str[ptr];

            switch (c)
            {
                case ParseCharacterTokens.Hyphen:
                    continue;
                case ParseCharacterTokens.ForwardSlash:
                    ++ptr;
                    break;
            }

            break; 
        }

        if (ptr == 2 && str.Length == 2)
        {
            return new SymbolSyntax(
                SymbolSyntaxType.ArgumentTerminator, 
                text: ParseSymbols.ArgumentTerminator);
        }

        var prefix = ptr > 0 ? str.Substring(0, ptr) : string.Empty;
        var nonIdentifier = (ptr < str.Length && !char.IsLetterOrDigit(str[ptr])) ||
                            (ptr < str.Length - 1 && !char.IsLetterOrDigit(str[^1]));

        // Short-circuit #1
        if (nonIdentifier)
        {
            return new SymbolSyntax(SymbolSyntaxType.NonIdentifier, str);
        }
        
        var last = char.MinValue;
        var identifierStartIndex = ptr;
        
        for (; ptr < str.Length; ptr++)
        {
            c = str[ptr];

            if (!nonIdentifier && c is ParseCharacterTokens.Colon or ParseCharacterTokens.Equal)
            {
                break;
            }

            try
            {
                if (char.IsLetterOrDigit(c))
                {
                    continue;
                }

                // Allow breaks in words
                if (c == ParseCharacterTokens.Hyphen && last != ParseCharacterTokens.Hyphen)
                    continue;

                nonIdentifier = true;
            }
            finally
            {
                last = c;
            }
        }
        
        // Short-circuit #2
        if (nonIdentifier)
        {
            return new SymbolSyntax(SymbolSyntaxType.NonIdentifier, str);
        }

        var syntaxType = prefix switch
        {
            ParseSymbols.PosixPrefix => SymbolSyntaxType.PosixPrefixed,
            ParseSymbols.GnuPrefix => SymbolSyntaxType.GnuPrefixed,
            ParseSymbols.ForwardSlashPrefix => SymbolSyntaxType.SlashPrefixed,
            _ => SymbolSyntaxType.Simple
        };

        var operandAssignmentTokenIndex = ptr < str.Length - 1
            ? ptr
            : -1;

        var identifiers = new List<string>(4);
        var identifierEndIndex = operandAssignmentTokenIndex > -1
            ? operandAssignmentTokenIndex
            : str.Length;

        if (syntaxType == SymbolSyntaxType.PosixPrefixed)
        {
            // Each character is an identifier
            for (var i = identifierStartIndex; i != identifierEndIndex; i++)
            {
                identifiers.Add($"{str[i]}");
            }
        }
        else
        {
            identifiers.Add(str.Substring(
                identifierStartIndex,
                identifierEndIndex-identifierStartIndex));
        }

        var operandAssignmentToken = operandAssignmentTokenIndex > -1
            ? str[operandAssignmentTokenIndex]
            : char.MinValue;

        var operandValue = operandAssignmentTokenIndex > -1
            ? str.Substring(operandAssignmentTokenIndex + 1)
            : string.Empty;

        var prefixedIdentifiers = identifiers
            .Select(id => $"{prefix}{id}")
            .ToArray();

        return new SymbolSyntax(
            syntaxType,
            str,
            prefix,
            prefixedIdentifiers,
            identifiers.ToArray(),
            operandAssignmentToken,
            operandValue);
    }
}