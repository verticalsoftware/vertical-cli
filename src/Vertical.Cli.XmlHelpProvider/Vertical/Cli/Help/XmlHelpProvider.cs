using System.Xml.Serialization;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

public sealed class XmlHelpProvider : IHelpProvider
{
    /// <inheritdoc />
    public string? GetCommandDescription(ICommandDefinition command)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public string GetApplicationName(ICommandDefinition rootCommand)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public string GetCommandName(ICommandDefinition command)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Predicate<SymbolDefinition> SymbolSelector { get; }

    /// <inheritdoc />
    public Predicate<ICommandDefinition> CommandSelector { get; }

    /// <inheritdoc />
    public string? GetCommandUsageGrammar(ICommandDefinition command, IReadOnlyCollection<ICommandDefinition> subCommands)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public string? GetArgumentsUsageGrammar(ICommandDefinition command, IEnumerable<SymbolDefinition> symbols)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public string? GetOptionsUsageGrammar(ICommandDefinition command, IEnumerable<SymbolDefinition> symbols)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public string? GetSymbolDescription(SymbolDefinition symbol)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public string GetSymbolGrammar(SymbolDefinition symbol)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public string GetSymbolSortKey(SymbolDefinition symbol)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public string? GetSymbolArgumentName(SymbolDefinition symbol)
    {
        throw new NotImplementedException();
    }
}