namespace Vertical.Cli.Configuration;

internal static class CommandDefinitionExtensions
{
    internal static bool HasSubCommands(this ICommandDefinition command) => command.SubCommandIdentities.Any();

    internal static IEnumerable<SymbolDefinition> GetSymbols(this ICommandDefinition command) =>
        command.Symbols.Where(symbol => symbol.Scope is SymbolScope.Self or SymbolScope.SelfAndDescendents);

    internal static IEnumerable<SymbolDefinition> GetArgumentSymbols(this ICommandDefinition command) =>
        command.GetSymbols().Where(symbol => symbol.Type == SymbolType.Argument);
    
    internal static IEnumerable<SymbolDefinition> GetOptionOrSwitchSymbols(this ICommandDefinition command) =>
        command.GetSymbols().Where(symbol => symbol.Type is SymbolType.Option or SymbolType.Switch);
}