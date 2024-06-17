﻿using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration;

public partial class CliCommand<TModel, TResult>
{
    /// <inheritdoc cref="CliCommand"/>>
    internal override void VerifyConfiguration(ICollection<string> messages)
    {
        VerifySymbolNameSyntax(messages);
        VerifySymbolIdentifies(messages);
        VerifySymbolBindings(messages);
        VerifySymbolArity(messages);
        VerifyHandler(messages);

        foreach (var subCommand in Commands)
        {
            subCommand.VerifyConfiguration(messages);
        }
    }

    private void VerifyHandler(ICollection<string> messages)
    {
        // E.g., model=Empty
        if (Symbols.Count == 0)
            return;
        
        var requiresHandler = Symbols.Any(symbol => symbol.Scope is CliScope.Self or CliScope.SelfAndDescendants);

        switch (requiresHandler)
        {
            case true when _handler is null:
                messages.Add($"Command {PrimaryIdentifier}: Self or descendant scoped symbols defined, but handler not provided.");
                break;
            
            case false when _handler is not null:
                messages.Add($"Command {PrimaryIdentifier}: Descendant only scoped symbols defined, but handler provided.");
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
            .Where(symbol => symbol.Type == SymbolType.Argument)
            .Reverse()
            .Skip(1);

        var zeroOrOneSymbols = 0;

        foreach (var symbol in symbols)
        {
            var arity = symbol.Arity;

            if (arity.MaxCount == null || arity.MinCount != arity.MaxCount)
            {
                messages.Add($"{FormatSymbol(symbol)}: Single variadic argument must appear last.");
                return;
            }

            if (arity == Arity.ZeroOrOne && ++zeroOrOneSymbols > 1)
            {
                messages.Add($"{FormatSymbol(symbol)}: Only a single (0,1) arity argument allowed.");
            }
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