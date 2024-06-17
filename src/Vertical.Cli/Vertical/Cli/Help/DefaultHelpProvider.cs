using System.Diagnostics.CodeAnalysis;
using System.Text;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Provides methods to create and display basic formatted help content.
/// </summary>
public sealed class DefaultHelpProvider : IHelpProvider
{
    private static readonly DefaultHelpProviderOptions DefaultOptions = new()
    {
        RenderWidth = Console.WindowWidth
    };
    
    /// <summary>
    /// Gets an instance of this type.
    /// </summary>
    public static readonly IHelpProvider Instance = new DefaultHelpProvider(DefaultOptions);
    
    private readonly DefaultHelpProviderOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultHelpProvider"/> type.
    /// </summary>
    /// <param name="options">Options</param>
    public DefaultHelpProvider(DefaultHelpProviderOptions options) => _options = options;

    /// <inheritdoc />
    public string GetContent(CliCommand command)
    {
        var sb = new StringBuilder(5000);
        var indent = Math.Max(0, _options.IndentSpaces);
        var tabs = (T1: new string(' ', indent), T2: new string(' ', indent * 2));
        var symbols = command.AggregateSymbols().ToArray();

        BuildDescriptionSection(sb, command, tabs);
        BuildUsageSection(sb, command, tabs);
        BuildSubCommandsSection(sb, command, tabs);
        BuildArgumentsSection(sb, symbols, tabs);
        BuildOptionsSection(sb, command, symbols, tabs);

        return sb.ToString();
    }

    private void BuildDescriptionSection(StringBuilder sb, CliCommand command, (string T1, string T2) tabs)
    {
        if (!TryGetContent(command, out var content))
            return;

        sb.AppendLine("Description:");
        sb.Append(tabs.T1);
        Helpers.AppendWrapped(sb, content, _options.RenderWidth, tabs.T1, true);
    }

    private void BuildUsageSection(StringBuilder sb, 
        CliCommand command, 
        (string T1, string T2) tabs)
    {
        if (sb.Length > 0)
        {
            sb.AppendLine();
        }

        sb.AppendLine("Usage:");
        BuildUsage(sb, command, tabs);
        foreach (var subCommand in command.Commands)
        {
            BuildUsage(sb, subCommand, tabs, command.PrimaryIdentifier);
        }
    }

    private void BuildUsage(
        StringBuilder sb,
        CliCommand command,
        (string T1, string T2) tabs,
        string? parentIdentifier = null)
    {
        sb.Append(tabs.T1);
        if (parentIdentifier != null)
            sb.Append(parentIdentifier + ' ');
        sb.Append(command.PrimaryIdentifier);
        BuildUsageOperands(sb, command);
    }

    private void BuildUsageOperands(
        StringBuilder sb,
        CliCommand command)
    {
        foreach (var symbol in command.Symbols.Where(s => s.Type == SymbolType.Argument))
        {
            sb.Append(' ');
            BuildArityEnclosedNotation(sb, symbol);
        }

        if (command.Symbols.Any(s => s.Type is SymbolType.Option or SymbolType.Switch))
        {
            sb.Append(" [Options]");
        }

        sb.AppendLine();
    }

    private void BuildSubCommandsSection(StringBuilder sb, CliCommand command, (string T1, string T2) tabs)
    {
        var subCommands = command.Commands.Where(cmd => !cmd.IsActionSwitch).ToArray();
        if (subCommands.Length == 0)
            return;

        sb.AppendLine();
        sb.AppendLine("Commands:");
        foreach (var subCommand in subCommands)
        {
            sb.AppendLine($"{tabs.T1}{subCommand.PrimaryIdentifier}");
            if (!TryGetContent(subCommand, out var content))
                continue;
            Helpers.AppendWrapped(sb, content, _options.RenderWidth, tabs.T1, appendNewLine: true);
        }
    }

    private void BuildArgumentsSection(StringBuilder sb, CliSymbol[] symbols, (string T1, string T2) tabs)
    {
        var arguments = symbols.Where(sym => sym.Type == SymbolType.Argument).ToArray();
        if (arguments.Length == 0)
            return;

        var wrappingWidth = _options.RenderWidth - 2 * _options.IndentSpaces;
        sb.AppendLine();
        sb.AppendLine("Arguments:");
        foreach (var argument in arguments)
        {
            sb.Append(tabs.T1);
            BuildOperandNotation(sb, argument);
            sb.AppendLine();
            if (!TryGetContent(argument, out var content))
                continue;
            
            sb.Append(tabs.T2);
            Helpers.AppendWrapped(sb, content, wrappingWidth, tabs.T2, appendNewLine: true);
        }
    }

    private void BuildOptionsSection(StringBuilder sb,
        CliCommand command,
        IEnumerable<CliSymbol> symbols, 
        (string T1, string T2) tabs)
    {
        var options = symbols
            .Where(sym => sym.Type is SymbolType.Option or SymbolType.Switch)
            .ToArray();

        var actions = command.Commands.Where(cmd => cmd.IsActionSwitch).ToArray();

        if (options.Length + actions.Length == 0)
            return;

        var wrappingWidth = _options.RenderWidth - 2 * _options.IndentSpaces;
        sb.AppendLine();
        sb.AppendLine("Options:");
        foreach (var option in options)
        {
            sb.Append(tabs.T1);
            sb.Append(string.Join(", ", option.Names));
            if (!(option.ValueType == typeof(bool) || option.ValueType == typeof(bool?)))
            {
                sb.Append(" <");
                BuildOperandNotation(sb, option);
                sb.AppendLine(">");
            }
            
            if (!TryGetContent(option, out var content))
                continue;

            sb.Append(tabs.T2);
            Helpers.AppendWrapped(sb, content, wrappingWidth, tabs.T2, appendNewLine: true);
        }

        foreach (var action in actions)
        {
            sb.Append(tabs.T1);
            sb.AppendLine(string.Join(", ", action.Names));
            if (!TryGetContent(action, out var content))
                continue;

            sb.Append(tabs.T2);
            Helpers.AppendWrapped(sb, content, wrappingWidth, tabs.T2, appendNewLine: true);
        }
    }

    private void BuildArityEnclosedNotation(StringBuilder sb, CliSymbol symbol)
    {
        var arityNotation = symbol.Arity.MinCount > 0 ? "<>" : "[]";
        sb.Append(arityNotation[0]);
        BuildOperandNotation(sb, symbol);
        sb.Append(arityNotation[1]);
    }

    private void BuildOperandNotation(StringBuilder sb, CliSymbol symbol)
    {
        _options.OperandNameFormatter(sb, symbol);
        if (symbol.Arity.MaxCount.GetValueOrDefault(0) > 1)
            sb.Append("...");
    }

    private bool TryGetContent(CliPrimitive obj, [NotNullWhen(true)]out string? str)
    {
        str = _options.ContentProvider.GetContent(obj) ?? string.Empty;
        return !string.IsNullOrWhiteSpace(str);
    }
}