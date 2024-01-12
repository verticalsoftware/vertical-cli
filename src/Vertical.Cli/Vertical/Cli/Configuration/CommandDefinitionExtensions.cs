using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Configuration;

internal static class CommandDefinitionExtensions
{
    /// <summary>
    /// Gets the root command.
    /// </summary>
    /// <param name="command">The subject instance.</param>
    /// <returns>The root command, or the given instance if it is the root command.</returns>
    public static ICommandDefinition GetRootCommand(this ICommandDefinition command)
    {
        Guard.IsNotNull(command);
        
        while (true)
        {
            if (command.Parent == null)
                return command;

            command = command.Parent;
        }
    }

    /// <summary>
    /// Gets the symbols inherited by this instance.
    /// </summary>
    /// <param name="command">The subject command.</param>
    /// <returns></returns>
    public static IEnumerable<SymbolDefinition> GetInheritedSymbols(this ICommandDefinition command)
    {
        Guard.IsNotNull(command);
        
        return command
            .GetContainingCommands()
            .SelectMany(subject => subject.Symbols.Where(symbol => symbol.Scope is
                SymbolScope.ParentAndDescendents or SymbolScope.Descendents));
    }

    /// <summary>
    /// Gets any symbol that has visibility in this instance.
    /// </summary>
    /// <param name="command">The subject command.</param>
    /// <returns>A collection of <see cref="SymbolDefinition"/> objects.</returns>
    public static IEnumerable<SymbolDefinition> GetAllSymbols(this ICommandDefinition command)
    {
        Guard.IsNotNull(command);
        
        return command
            .GetInheritedSymbols()
            .Concat(command.Symbols
                .Where(symbol => symbol.Scope is SymbolScope.Parent or SymbolScope.ParentAndDescendents));
    }

    /// <summary>
    /// Gets the commands that contain this instance.
    /// </summary>
    /// <param name="command">The subject command.</param>
    /// <returns>A collection of <see cref="SymbolDefinition"/> object that begin with the root command
    /// and end with this instance's parent command.</returns>
    public static IEnumerable<ICommandDefinition> GetContainingCommands(this ICommandDefinition command)
    {
        return Enumerate(command.Parent).Reverse();
        
        static IEnumerable<ICommandDefinition> Enumerate(ICommandDefinition? subject)
        {
            while (subject != null)
            {
                yield return subject;
                subject = subject.Parent;
            }
        }
    }

    public static string GetPathString(this ICommandDefinition command)
    {
        return string.Join(" -> ", command.EnumerateToRoot().Reverse().Select(cmd => cmd.Id));
    }

    private static IEnumerable<ICommandDefinition> EnumerateToRoot(
        this ICommandDefinition? command)
    {
        for (; command != null; command = command.Parent)
        {
            yield return command;
        }
    }
}