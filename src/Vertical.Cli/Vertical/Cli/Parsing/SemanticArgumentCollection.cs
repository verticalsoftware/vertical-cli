using System.Collections;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents a collection of <see cref="SemanticArgument"/> objects parsed from syntax input.
/// </summary>
public class SemanticArgumentCollection : IEnumerable<SemanticArgument>
{
    private readonly SemanticArgument[] _expandedArguments;
    private readonly bool[] _flags;

    internal SemanticArgumentCollection(
        IEnumerable<SymbolDefinition> symbols,
        IReadOnlyList<SymbolSyntax> arguments)
    {
        _expandedArguments = ExpandSymbols(symbols, arguments).ToArray();
        _flags = Enumerable.Range(0, _expandedArguments.Length).Select(_ => true).ToArray();
    }

    /// <summary>
    /// Gets available arguments that match any identifiers of the given symbol.
    /// </summary>
    /// <param name="symbol">The symbol to match.</param>
    /// <returns>A collection of <see cref="SemanticArgument"/> instances.</returns>
    public IEnumerable<SemanticArgument> GetOptionArguments(SymbolDefinition symbol)
    {
        Guard.IsNotNull(symbol);

        return Unaccepted
            .Where(argument => ReferenceEquals(symbol, argument.OptionSymbol))
            .ToArray();
    }

    /// <summary>
    /// Gets available arguments for the given symbol.
    /// </summary>
    /// <param name="symbol">The argument symbol.</param>
    /// <returns>A collection of <see cref="SemanticArgument"/> instances.</returns>
    /// <remarks>
    /// This method will only return as many arguments as the symbol's arity allows.
    /// </remarks>
    public IEnumerable<SemanticArgument> GetValueArguments(SymbolDefinition symbol)
    {
        Guard.IsNotNull(symbol);

        var maxCount = symbol.Arity.MaxCount.GetValueOrDefault(int.MaxValue);

        return Unaccepted
            .Take(maxCount)
            .ToArray();
    }

    /// <summary>
    /// Gets the arguments that have not been accepted.
    /// </summary>
    public IEnumerable<SemanticArgument> Unaccepted => _expandedArguments
        .Where(arg => _flags[arg.OrdinalPosition]);

    private IEnumerable<SemanticArgument> ExpandSymbols(
        IEnumerable<SymbolDefinition> symbols,
        IReadOnlyList<SymbolSyntax> arguments)
    {
        var symbolDictionary = symbols
            .Where(symbol => symbol.Kind != SymbolKind.Argument)
            .SelectMany(symbol => symbol.Identities.Select(id => (id, symbol)))
            .ToDictionary(item => item.id, item => item.symbol);

        var position = 0;
        var terminated = false;

        for(var c = 0; c < arguments.Count; c++)
        {
            var syntax = arguments[c];
            var next = c < arguments.Count - 1 ? arguments[c + 1] : null;
            
            switch (syntax)
            {
                case not null when terminated:
                    yield return new SemanticArgument(Remove, syntax, position++, terminated: true);
                    break;
                    
                case { Type: SymbolSyntaxType.ArgumentTerminator } when !terminated:
                    terminated = true;
                    continue;
                
                case { Type: SymbolSyntaxType.Simple or SymbolSyntaxType.NonIdentifier }:
                    yield return new SemanticArgument(Remove, syntax, position++);
                    break;
                    
                case { Type: SymbolSyntaxType.PosixPrefixed, Identifiers.Length: > 1 }:
                    foreach (var shortIdentity in syntax.Identifiers.SkipLast(1))
                    {
                        yield return new SemanticArgument(
                            Remove,
                            SymbolSyntax.Parse(shortIdentity), 
                            position++,
                            symbolDictionary.GetValueOrDefault(shortIdentity));
                    }

                    yield return CreateOptionArgument(
                        symbolDictionary, 
                        SymbolSyntax.Parse(syntax.Identifiers.Last()), 
                        next, 
                        position++);
                    break;
                
                default:
                    yield return CreateOptionArgument(symbolDictionary, syntax!, next, position++);
                    break;
            }
        }
    }

    private SemanticArgument CreateOptionArgument(
        Dictionary<string, SymbolDefinition> symbolDictionary,
        SymbolSyntax syntax, 
        SymbolSyntax? next, 
        int position)
    {
        var symbol = symbolDictionary.GetValueOrDefault(syntax.Identifiers[0]);
        
        if (syntax.HasOperand)
        {
            return new SemanticArgument(Remove, syntax, position, symbol);
        }

        var isCandidateOperand = symbol != null && 
                                 next is not null &&
                                 !(next.Identifiers.Length == 1 && symbolDictionary.ContainsKey(next.Identifiers[0]));
        
        return isCandidateOperand
            ? new SemanticArgument(Remove, syntax, position, symbol, next, position + 1)
            : new SemanticArgument(Remove, syntax, position, symbol);
    }

    private void Remove(int position) => _flags[position] = false;

    /// <inheritdoc />
    public IEnumerator<SemanticArgument> GetEnumerator() => _expandedArguments
        .Cast<SemanticArgument>()
        .GetEnumerator();

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}