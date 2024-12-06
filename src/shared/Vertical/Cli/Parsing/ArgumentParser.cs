namespace Vertical.Cli.Parsing;

/// <summary>
/// Parses an argument collection.
/// </summary>
public static class ArgumentParser
{
    /// <summary>
    /// Parses a collection of arguments to <see cref="ArgumentSyntax"/>
    /// </summary>
    /// <param name="arguments">Collection of arguments.</param>
    /// <returns><see cref="List{T}"/></returns>
    public static List<ArgumentSyntax> Parse(IReadOnlyCollection<string> arguments)
    {
        var list = new List<ArgumentSyntax>(32);

        list.AddRange(arguments
            .TakeWhile(argument => argument != "--")
            .Select(ArgumentSyntax.Parse));
        
        list.AddRange(arguments
            .Skip(list.Count + 1)
            .Select(argument => new ArgumentSyntax(OptionPrefixType.None, argument)));

        return list;
    }
}