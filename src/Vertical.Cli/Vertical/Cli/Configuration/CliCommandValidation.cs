using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration;

public partial class CliCommand<TModel>
{
    /// <inheritdoc cref="CliCommand"/>>
    internal override void VerifyConfiguration(ICollection<string> messages)
    {
        VerifySymbolNameSyntax(messages);
        VerifySymbolIdentifies(messages);
        VerifySymbolBindings(messages);
        VerifySymbolArity(messages);
        VerifyHandler(messages);

        foreach (var subCommand in SubCommands)
        {
            subCommand.VerifyConfiguration(messages);
        }
    }

    private void VerifyHandler(ICollection<string> messages)
    {
        // E.g., model=Empty
        if (Symbols.Count == 0)
            return;

        var requiresHandler = Symbols
            .Any(symbol => symbol is
            {
                Scope: CliScope.Self or CliScope.SelfAndDescendants,
                Type: not SymbolType.Action
            });

        switch (requiresHandler)
        {
            case true when _handler is null:
                messages.Add($"Command {PrimaryIdentifier}: Scoped symbols defined, but handler not provided.");
                break;
            
            case false when _handler is not null:
                messages.Add($"Command {PrimaryIdentifier}: No scoped symbols defined, but handler provided.");
                break;
        }
    }

    private void VerifySymbolNameSyntax(ICollection<string> messages)
    {
        var items = Symbols
            .Where(symbol => symbol.Type != SymbolType.Argument)
            .SelectMany(symbol => symbol.Names.Select(name => (symbol, name)));

        foreach (var (symbol, name) in items)
        {
            if (ArgumentSyntax.Parse(name).PrefixType != OptionPrefixType.None)
                continue;
            var names = string.Join(',', symbol.Names);
            messages.Add($"Symbol {FormatSymbol(symbol)}: Invalid option/switch identifier \"{name}\".");
        }
    }

    private void VerifySymbolIdentifies(ICollection<string> messages)
    {
        var groups = this
            .AggregateSymbols()
            .SelectMany(symbol => symbol.Names.Select(name => (symbol, name)))
            .GroupBy(item => item.name, item => item.symbol)
            .Where(grouping => grouping.Count() > 1);

        foreach (var group in groups)
        {
            var symbols = string.Join(',', group.Select(symbol => FormatSymbol(symbol, shortForm: true)));
            messages.Add($"Command {PrimaryIdentifier}: Duplicate identifier \"{group.Key}\" used by {symbols}.");
        }     
    }

    private void VerifySymbolBindings(ICollection<string> messages)
    {
        var groups = this
            .AggregateSymbols()
            .Select(symbol => (symbol, binding: symbol.BindingName))
            .GroupBy(item => item.binding, item => item.symbol)
            .Where(grouping => grouping.Count() > 1);

        foreach (var group in groups)
        {
            var symbols = string.Join(',', group.Select(symbol => FormatSymbol(symbol, shortForm: true)));
            messages.Add($"Command {PrimaryIdentifier}: Duplicate model binding \"{group.Key}\" used by {symbols}.");
        }

        foreach (var symbol in Symbols)
        {
            var bindingType = symbol.ValueType;
            var isCollection =
                bindingType is { Namespace: "System.Collections.Generic" or "System.Collections.Immutable" }
                ||
                bindingType.IsArray;

            if (!isCollection && symbol.Arity is { MaxCount: null or > 1 })
            {
                messages.Add($"{FormatSymbol(symbol)}: Arity ({symbol.Arity}) is not compatible with model type.");
            }
        }
    }

    private void VerifySymbolArity(ICollection<string> messages)
    {
        var symbols = Symbols
            .Where(symbol => symbol.Type == SymbolType.Argument && symbol.Arity.MinCount != symbol.Arity.MaxCount)
            .ToArray();

        switch (symbols.Length)
        {
            case 0:
            case 1:
                return;
            case {} when !ReferenceEquals(symbols.Last(), Symbols.Last()):
                messages.Add($"{FormatSymbol(symbols.Last())}: Single variadic positional argument must be defined last.");
                break;
            default:
                messages.Add($"Command {PrimaryIdentifier}: Only one variadic positional argument can be defined.");
                break;
        }
    }

    private string FormatSymbol(CliSymbol symbol, bool shortForm = false)
    {
        var command = shortForm ? string.Empty : $"{PrimaryIdentifier}/";
        
        return symbol switch
        {
            { Type: SymbolType.Argument } => $"Argument {command}[->\"{symbol.BindingName}\"]",
            { Type: SymbolType.Option } => $"Option {command}[{string.Join(',', symbol.Names)} -> {symbol.BindingName}]",
            _ => $"Switch {command}[{string.Join(',', symbol.Names)} -> {symbol.BindingName}]"
        };
    }
}