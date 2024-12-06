using System.Collections;
using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;
using Vertical.Cli.Routing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents a lookup of binding values.
/// </summary>
public sealed class BindingValuesLookup : ILookup<string, ArgumentSyntax>
{
    private readonly ILookup<string, ArgumentSyntax> lookup;

    private BindingValuesLookup(ParameterCollection parameters, ILookup<string, ArgumentSyntax> lookup)
    {
        this.lookup = lookup;
        Parameters = parameters;
    }

    /// <inheritdoc />
    public IEnumerator<IGrouping<string, ArgumentSyntax>> GetEnumerator() => lookup.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public bool Contains(string key) => lookup.Contains(key);

    /// <inheritdoc />
    public int Count => lookup.Count;

    /// <inheritdoc />
    public IEnumerable<ArgumentSyntax> this[string key] => lookup[key];

    internal ParameterCollection Parameters { get; }
    
    internal static BindingValuesLookup Create(CliApplication application, RouteTarget routeTarget)  
    {
        var parameters = ParameterCollection.Create(application, routeTarget);
        var valueEntries = new List<(string key, ArgumentSyntax value)>(32);
        var argumentList = new LinkedList<ArgumentSyntax>(routeTarget.Arguments);

        TakeOptions(routeTarget.Route, parameters, argumentList, valueEntries);
        TakeArguments(parameters, argumentList, valueEntries);

        return new BindingValuesLookup(
            parameters,
            valueEntries.ToLookup(t => t.key, t => t.value));
    }

    private static void TakeArguments(
        ParameterCollection parameters, 
        LinkedList<ArgumentSyntax> argumentList, 
        List<(string key, ArgumentSyntax value)> entries)
    {
        var argumentParameters = new Queue<CliParameter>(parameters
            .Where(parameter => parameter.SymbolKind == SymbolKind.Argument)
            .OrderBy(parameter => parameter.Index));

        if (argumentParameters.Count == 0)
            return;

        for (var node = argumentList.First; node != null && argumentParameters.TryDequeue(out var parameter);)
        {
            TakeArguments(entries, 
                parameter,
                consumed => argumentList.Remove(consumed, consumed.Next),
                ref node);
        }
    }

    private static void TakeArguments(List<(string key, ArgumentSyntax value)> entries, 
        CliParameter parameter,
        Func<LinkedListNode<ArgumentSyntax>, LinkedListNode<ArgumentSyntax>?> delete,
        ref LinkedListNode<ArgumentSyntax>? node)
    {
        var collected = 0;
        var maxCount = parameter.Arity.MaxCount.GetValueOrDefault(int.MaxValue);

        while (node != null && collected < maxCount)
        {
            entries.Add((parameter.BindingName, node.Value));
            node = delete(node);
            ++collected;
        }
    }

    private static void TakeOptions(
        RouteDefinition route,
        ParameterCollection parameters,
        LinkedList<ArgumentSyntax> argumentList, 
        List<(string key, ArgumentSyntax value)> entries)
    {
        for (var node = argumentList.First; node != null;)
        {
            var argument = node.Value;

            if (argument.PrefixType == OptionPrefixType.None)
            {
                node = node.Next;
                continue;
            }

            if (!parameters.TryGetByIdentifier(argument.IdentifierSymbol, out var parameter))
            {
                throw Exceptions.IdentifierNotFound(argument);
            }

            node = argumentList.Remove(node, node.Next);

            if (parameter.SymbolKind == SymbolKind.Switch)
            {
                ValidateSwitchArgument(route, parameter, argument);
                entries.Add((parameter.BindingName, argument));
                continue;
            }

            if (argument.OperandValue != string.Empty)
            {
                entries.Add((parameter.BindingName, argument));
                continue;
            }

            if (node?.Value is { PrefixType: OptionPrefixType.None } operandArgument)
            {
                entries.Add((parameter.BindingName, operandArgument));
                node = argumentList.Remove(node, node.Next);
                continue;
            }

            throw Exceptions.OptionValueNotProvided(route, parameter, argument);
        }
    }

    private static void ValidateSwitchArgument(RouteDefinition route, CliParameter parameter, ArgumentSyntax argument)
    {
        if (argument.OperandValue == string.Empty)
            return;

        throw Exceptions.SwitchValueProvided(route, parameter, argument);
    }
}