namespace Vertical.Cli.Parsing;

/// <summary>
/// Generic utility extensions for an argument list.
/// </summary>
public static class ArgumentSyntaxListExtensions
{
    /// <summary>
    /// Gets whether an argument is in the list that matches any given identifiers.
    /// </summary>
    /// <param name="arguments">Argument list</param>
    /// <param name="names">One or more names to match</param>
    /// <returns>Boolean</returns>
    public static bool HasOption(this IReadOnlyList<ArgumentSyntax> arguments, string[] names)
    {
        return arguments.GetOption(names) != null;
    }
    
    /// <summary>
    /// Gets the first option that matches any provided identifiers.
    /// </summary>
    /// <param name="arguments">Argument list</param>
    /// <param name="names">One or more names to match</param>
    /// <returns><see cref="ArgumentSyntax"/></returns>
    public static ArgumentSyntax? GetOption(this IReadOnlyList<ArgumentSyntax> arguments, string[] names)
    {
        var identifiers = names
            .Select(ArgumentSyntax.Parse)
            .ToArray();

        return arguments.FirstOrDefault(arg => identifiers.Any(id => id.PrefixType == arg.PrefixType 
                                                                     && id.IdentifierName == arg.IdentifierName));
    }

    /// <summary>
    /// Gets the operand for the first option that matches any provided identifiers.
    /// </summary>
    /// <param name="arguments">Argument list</param>
    /// <param name="names">One or more names to match</param>
    /// <returns>The value of the first matched option's operand or trailing argument value.</returns>
    public static string? GetOperand(this IReadOnlyList<ArgumentSyntax> arguments, string[] names)
    {
        var identifiers = names
            .Select(ArgumentSyntax.Parse)
            .ToArray();

        for (var c = 0; c < arguments.Count; c++)
        {
            var argument = arguments[c];

            if (!identifiers.Any(id => id.PrefixType == argument.PrefixType
                                       && id.IdentifierName == argument.IdentifierName))
                continue;

            if (!string.IsNullOrWhiteSpace(argument.OperandValue))
                return argument.OperandValue;

            if (c < arguments.Count - 1 && arguments[c + 1].PrefixType == OptionPrefixType.None)
                return arguments[c + 1].Text;
        }

        return null;
    }
}