using Vertical.Cli.Internal;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Extends functionality of commands and symbols.
/// </summary>
public static class CliExtensions
{
    /// <summary>
    /// Gets a path string.
    /// </summary>
    /// <param name="symbol">Symbol</param>
    /// <param name="separator">Separator used to concatenate identifiers.</param>
    /// <returns>
    /// A string that represents the full path of the given symbol, starting with the root command.
    /// </returns>
    public static string GetPathString(this ICliSymbol symbol, char separator = '/')
    {
        var path = symbol
            .SelectRecursive(item => (item.PrimaryIdentifier, item.ParentSymbol))
            .Reverse();

        return string.Join(separator, path);
    }
    
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
        return command.SelectManyRecursive(current =>
        {
            Predicate<CliSymbol> filter = ReferenceEquals(command, current)
                ? symbol => symbol.Scope != CliScope.Descendants
                : symbol => symbol.Scope != CliScope.Self;

            return (
                current.Symbols.Where(item => filter(item)),
                current.Parent);
        });
    }

    /// <summary>
    /// Aggregates short tasks.
    /// </summary>
    /// <param name="command">Command</param>
    /// <returns>An enumeration of <see cref="ModelessTaskConfiguration"/> objects in the path.</returns>
    public static IEnumerable<ModelessTaskConfiguration> AggregateModelessTasks(this CliCommand command)
    {
        return command.SelectManyRecursive(current =>
        {
            Predicate<ModelessTaskConfiguration> filter = ReferenceEquals(command, current)
                ? symbol => symbol.Scope != CliScope.Descendants
                : symbol => symbol.Scope != CliScope.Self;

            return (
                current.ModelessTasks.Where(item => filter(item)),
                current.Parent);
        });
    }
}