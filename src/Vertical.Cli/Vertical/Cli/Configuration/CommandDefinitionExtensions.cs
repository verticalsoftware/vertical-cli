namespace Vertical.Cli.Configuration;

internal static class CommandDefinitionExtensions
{
    internal static ICommandDefinition GetRootCommand(this ICommandDefinition command)
    {
        while (true)
        {
            if (command.Parent == null)
                return command;

            command = command.Parent;
        }
    }

    internal static IEnumerable<SymbolDefinition> GetScopedSymbols(this ICommandDefinition command)
    {
        return command.Symbols.Where(symbol => symbol.Scope is SymbolScope.Parent or SymbolScope.ParentAndDescendents);
    }

    internal static IEnumerable<SymbolDefinition> GetInheritedSymbols(this ICommandDefinition command)
    {
        return command
            .GetContainingCommands()
            .SelectMany(subject => subject.Symbols.Where(symbol => symbol.Scope is
                SymbolScope.ParentAndDescendents or SymbolScope.Descendents));
    }

    internal static IEnumerable<SymbolDefinition> GetAllSymbols(this ICommandDefinition command)
    {
        return command.GetInheritedSymbols().Concat(command.GetInheritedSymbols());
    }

    internal static IEnumerable<ICommandDefinition> GetContainingCommands(this ICommandDefinition command)
    {
        return Enumerate(command).Reverse();
        
        static IEnumerable<ICommandDefinition> Enumerate(ICommandDefinition? subject)
        {
            while (subject != null)
            {
                yield return subject;
                subject = subject.Parent;
            }
        }
    }
    
    internal static string GetPathString(this ICommandDefinition command)
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