namespace Vertical.Cli.Configuration;

/// <summary>
/// Extends functionality of commands and symbols.
/// </summary>
public static class CliExtensions
{
    /// <summary>
    /// Gets the root options.
    /// </summary>
    /// <param name="command">Command in the path.</param>
    /// <returns><see cref="CliOptions"/></returns>
    public static CliOptions GetOptions(this CliCommand command)
    {
        var optionsRoot = (IOptionsRoot)command.SelectPath().First();
        return optionsRoot.Options;
    }
    
    /// <summary>
    /// Selects the path of the object starting with the root command.
    /// </summary>
    /// <param name="obj">Command or symbol.</param>
    /// <returns><see cref="IEnumerable{T}"/></returns>
    public static IEnumerable<CliPrimitive> SelectPath(this CliPrimitive obj)
    {
        return Select(obj).Reverse();
        
        static IEnumerable<CliPrimitive> Select(CliPrimitive? target)
        {
            for (; target != null; target = target.Parent)
                yield return target;
        }    
    }

    /// <summary>
    /// Constructs a path string.
    /// </summary>
    /// <param name="obj">Command or symbol.</param>
    /// <returns>
    /// A string composed of the identifiers in the path of <paramref name="obj"/>
    /// separated by <c>/</c>.
    /// </returns>
    public static string SelectPathString(this CliPrimitive obj) => string.Join(',', obj.SelectPath());
    
    /// <summary>
    /// Aggregates the symbols in the path.
    /// </summary>
    /// <param name="command">Command</param>
    /// <returns>An enumerable that emits each <see cref="CliSymbol"/></returns>
    public static IEnumerable<CliSymbol> AggregateSymbols(this CliCommand command)
    {
        return command.AggregateSymbolsInternal().Reverse();
    }
    
    private static IEnumerable<CliSymbol> AggregateSymbolsInternal(this CliCommand command)
    {
        foreach (var symbol in command
                     .Symbols
                     .Reverse()
                     .Where(sym => sym.Scope != CliScope.Descendants))
        {
            yield return symbol;
        }

        for (var parent = command.ParentCommand; parent != null; parent = parent.ParentCommand)
        {
            foreach (var symbol in parent
                         .Symbols
                         .Reverse()
                         .Where(sym => sym.Scope != CliScope.Self))
            {
                yield return symbol;
            }
        }
    }
}