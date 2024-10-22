using System.Diagnostics.CodeAnalysis;
using System.Text;
using Vertical.Cli.Configuration;
using Vertical.Cli.Internal;

namespace Vertical.Cli.Help;

/// <summary>
/// Provides methods to create and display basic formatted help content.
/// </summary>
public sealed class DefaultHelpProvider : IHelpProvider
{
    private record RenderInfo(
        CliCommand Target,
        CliSymbol[] Symbols,
        StringBuilder Buffer,
        int Width, 
        string TabX1, 
        string TabX2);
    
    /// <summary>
    /// Gets an instance of this type.
    /// </summary>
    public static readonly IHelpProvider Instance = new DefaultHelpProvider(new DefaultHelpOptions());
    
    private readonly DefaultHelpOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultHelpProvider"/> type.
    /// </summary>
    /// <param name="options">Options</param>
    public DefaultHelpProvider(DefaultHelpOptions options) => _options = options;

    /// <inheritdoc />
    public string GetContent(CliCommand command)
    {
        var indentSpaces = Math.Max(0, _options.IndentSpaces);
        var renderInfo = new RenderInfo(
            command,
            BuildSymbols(command),
            new StringBuilder(5000),
            _options.RenderWidth(),
            new string(' ', indentSpaces),
            new string(' ', indentSpaces * 2));
        
        BuildDescriptionSection(renderInfo);
        BuildUsageSection(renderInfo);
        BuildSubCommandsSection(renderInfo);
        BuildArgumentsSection(renderInfo);
        BuildOptionsSection(renderInfo);

        renderInfo.Buffer.AppendLine();
        
        return renderInfo.Buffer.ToString();
    }

    private void BuildDescriptionSection(RenderInfo renderInfo)
    {
        if (!TryGetContent(renderInfo.Target, out var content))
            return;

        var sb = renderInfo.Buffer;

        sb.AppendLine("Description:");
        sb.Append(renderInfo.TabX1);
        Helpers.AppendWrapped(sb, content, renderInfo.Width, renderInfo.TabX1, true);
    }

    private void BuildUsageSection(RenderInfo renderInfo)
    {
        var sb = renderInfo.Buffer;
        
        if (sb.Length > 0)
        {
            sb.AppendLine();
        }

        var fullName = string.Join(
            ' ',
            renderInfo.Target
                .SelectRecursive(source => (source.PrimaryIdentifier, source.Parent))
                .Reverse());

        sb.AppendLine("Usage:");
        BuildUsage(renderInfo, renderInfo.Target, fullName);
    }

    private void BuildUsage(
        RenderInfo renderInfo,
        CliCommand command,
        string fullName)
    {
        var sb = renderInfo.Buffer;

        sb.Append(renderInfo.TabX1);
        sb.Append(fullName);
        
        BuildUsageOperands(sb, command);
    }

    private void BuildUsageOperands(
        StringBuilder sb,
        CliCommand command)
    {
        if (command.SubCommands.Any())
        {
            var isStandAloneCommand = command.Symbols.Any(symbol => symbol.Scope != CliScope.Descendants);
            var commandNotation = GetArityNotation(!isStandAloneCommand);
            sb.Append(' ');
            sb.Append(commandNotation[0]);
            sb.Append("command");
            sb.Append(commandNotation[1]);
        }
        
        foreach (var symbol in command.Symbols.Where(s => s.Type == SymbolType.Argument &&
                                                          s.Scope != CliScope.Descendants))
        {
            sb.Append(' ');
            BuildArityEnclosedNotation(sb, symbol);
        }

        if (command.Symbols.Any(s => s.Type != SymbolType.Argument && s.Scope != CliScope.Descendants))
        {
            sb.Append(" [options]");
        }

        sb.AppendLine();
    }

    private void BuildSubCommandsSection(RenderInfo renderInfo)
    {
        var sb = renderInfo.Buffer;
        var subCommands = renderInfo
            .Target
            .SubCommands
            .ToArray();
        
        if (subCommands.Length == 0)
            return;

        var count = 0;
        sb.AppendLine();
        sb.AppendLine("Commands:");
        
        foreach (var subCommand in subCommands.Order(_options.NameComparer))
        {
            if (_options.DoubleSpace && ++count > 1)
                sb.AppendLine();
            
            sb.AppendLine($"{renderInfo.TabX1}{subCommand.PrimaryIdentifier}");
            if (!TryGetContent(subCommand, out var content))
                continue;
            sb.Append(renderInfo.TabX2);
            Helpers.AppendWrapped(sb, content, renderInfo.Width, renderInfo.TabX2, appendNewLine: true);
        }
    }

    private void BuildArgumentsSection(RenderInfo renderInfo)
    {
        var sb = renderInfo.Buffer;
        var arguments = renderInfo 
            .Symbols
            .Where(sym => sym.Type == SymbolType.Argument).ToArray();
        
        if (arguments.Length == 0)
            return;

        var count = 0;
        var wrappingWidth = renderInfo.Width - 2 * _options.IndentSpaces;
        sb.AppendLine();
        sb.AppendLine("Arguments:");
        
        foreach (var argument in arguments)
        {
            if (_options.DoubleSpace && ++count > 1)
                sb.AppendLine();
            
            sb.Append(renderInfo.TabX1);
            BuildOperandNotation(sb, argument, []);
            sb.AppendLine();
            if (!TryGetContent(argument, out var content))
                continue;
            
            sb.Append(renderInfo.TabX2);
            Helpers.AppendWrapped(sb, content, wrappingWidth, renderInfo.TabX2, appendNewLine: true);
        }
    }

    private void BuildOptionsSection(RenderInfo renderInfo)
    {
        var sb = renderInfo.Buffer;

        var items = renderInfo
            .Symbols
            .Where(sym => sym.Type != SymbolType.Argument)
            .Cast<ICliSymbol>()
            .Concat(renderInfo.Target.AggregateModelessTasks())
            .ToArray();

        if (items.Length == 0)
            return;

        var wrappingWidth = renderInfo.Width - 2 * _options.IndentSpaces;

        var optionGroups = BuildOptionGroups(items);

        foreach (var optionGroup in optionGroups)
        {
            var count = 0;

            sb.AppendLine();
            sb.AppendLine($"{optionGroup.Key}:");

            foreach (var item in optionGroup)
            {
                if (_options.DoubleSpace && ++count > 1)
                    sb.AppendLine();
            
                sb.Append(renderInfo.TabX1);
                sb.Append(string.Join(", ", item.Names));

                if (item is CliSymbol option && !(option.ValueType == typeof(bool) || option.ValueType == typeof(bool?)))
                {
                    sb.Append(' ');
                    BuildOperandNotation(sb, option, ['<','>']);
                }

                sb.AppendLine();
            
                if (!TryGetContent(item, out var content))
                    continue;

                sb.Append(renderInfo.TabX2);
                Helpers.AppendWrapped(sb, content, wrappingWidth, renderInfo.TabX2, appendNewLine: true);
            }
        }
    }

    private IEnumerable<IGrouping<string, ICliSymbol>> BuildOptionGroups(IEnumerable<ICliSymbol> symbols)
    {
        var groups = _options.OptionGroups ?? [];

        return symbols
            .GroupBy(symbol => symbol.OptionGroup ?? "Options")
            .OrderBy(group => Array.IndexOf(groups, group.Key));
    }

    private void BuildArityEnclosedNotation(StringBuilder sb, CliSymbol symbol)
    {
        var arityNotation = GetArityNotation(symbol.Arity.MinCount > 0);
        BuildOperandNotation(sb, symbol, arityNotation);
    }

    private static char[] GetArityNotation(bool required) => required ? ['<', '>'] : ['[', ']'];

    private void BuildOperandNotation(StringBuilder sb, CliSymbol symbol, char[] arityNotation)
    {
        if (arityNotation.Length == 2)
            sb.Append(arityNotation[0]);
        
        _options.OperandNameFormatter(sb, symbol);

        if (arityNotation.Length == 2)
            sb.Append(arityNotation[1]);
        
        if (symbol.Arity.MaxCount is null or > 1)
            sb.Append("...");
    }

    private static CliSymbol[] BuildSymbols(CliCommand command)
    {
        // Ensure help switch is displayed
        return command.AggregateSymbols().ToArray();
    }

    private bool TryGetContent(ICliSymbol obj, [NotNullWhen(true)]out string? str)
    {
        str = _options.ContentProvider.GetContent(obj) ?? string.Empty;
        return !string.IsNullOrWhiteSpace(str);
    }
}