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
        return command.Symbols.Where(symbol => symbol.Scope is SymbolScope.Self or SymbolScope.SelfAndDescendents);
    }

    internal static IEnumerable<SymbolDefinition> GetInheritedSymbols(this ICommandDefinition command)
    {
        return command
            .GetContainingCommands()
            .SelectMany(subject => subject.Symbols.Where(symbol => symbol.Scope is
                SymbolScope.SelfAndDescendents or SymbolScope.Descendents));
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
}