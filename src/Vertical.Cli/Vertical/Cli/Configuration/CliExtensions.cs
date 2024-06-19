namespace Vertical.Cli.Configuration;

/// <summary>
/// Extends functionality of commands and symbols.
/// </summary>
public static class CliExtensions
{
    /// <summary>
    /// Gets the root options.
    /// </summary>
    /// <param name="command">Command</param>
    /// <returns><see cref="CliOptions"/></returns>
    public static CliOptions GetOptions(this CliCommand command) => command.GetRootCommand().Options;

    /// <summary>
    /// Resolves the root command.
    /// </summary>
    /// <param name="command">Command instance.</param>
    /// <returns><see cref="IRootCommand"/></returns>
    public static IRootCommand GetRootCommand(this CliCommand command)
    {
        var target = command;
        while (target.Parent != null)
        {
            target = target.Parent;
        }

        return (IRootCommand)target;
    }
    
    /// <summary>
    /// Aggregates the symbols in the path.
    /// </summary>
    /// <param name="command">Command</param>
    /// <returns>An enumerable that emits each <see cref="CliSymbol"/></returns>
    public static IEnumerable<CliSymbol> AggregateSymbols(this CliCommand command)
    {
        for (var current = command; current != null; current = current.Parent)
        {
            Predicate<CliSymbol> filter = ReferenceEquals(command, current)
                ? symbol => symbol.Scope != CliScope.Descendants
                : symbol => symbol.Scope != CliScope.Self;

            foreach (var symbol in current.Symbols.Where(item => filter(item)))
                yield return symbol;
        }
    }

    /// <summary>
    /// Aggregates short tasks.
    /// </summary>
    /// <param name="command">Command</param>
    /// <returns>An enumeration of <see cref="ModelessTaskConfiguration"/> objects in the path.</returns>
    public static IEnumerable<ModelessTaskConfiguration> AggregateModelessTasks(this CliCommand command)
    {
        foreach (var task in command.ModelessTasks.Where(t => t.Scope != CliScope.Descendants))
            yield return task;

        for (var target = command.Parent; target != null; target = target.Parent)
        {
            foreach (var task in target.ModelessTasks.Where(t => t.Scope != CliScope.Self))
                yield return task;
        }
    }
}