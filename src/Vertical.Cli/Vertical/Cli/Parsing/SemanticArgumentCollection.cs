using System.Collections;
using CommunityToolkit.Diagnostics;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents a collection of semantic arguments.
/// </summary>
public sealed class SemanticArgumentCollection : IEnumerable<SemanticArgument>
{
    private readonly IReadOnlyList<SemanticArgument> _expandedArguments;
    private readonly bool[] _flags;
    
    internal SemanticArgumentCollection(IReadOnlyList<SymbolSyntax> argumentSyntax)
    {
        _expandedArguments = ExpandArgumentSyntax(argumentSyntax);
        _flags = Enumerable.Range(0, _expandedArguments.Count).Select(_ => true).ToArray();
    }

    internal SemanticArgumentCollection(SemanticArgumentCollection other)
    {
        _expandedArguments = other._expandedArguments.ToArray();
        _flags = Enumerable.Range(0, _expandedArguments.Count).Select(_ => true).ToArray();
    }

    /// <summary>
    /// Gets available arguments that match any identifiers of the given symbol.
    /// </summary>
    /// <param name="symbol">The symbol to match.</param>
    /// <returns>A collection of <see cref="SemanticArgument"/> instances.</returns>
    public IEnumerable<SemanticArgument> GetOptionArguments(SymbolDefinition symbol)
    {
        Guard.IsNotNull(symbol);
        
        return InternalArguments
            .Where(argument => argument.ArgumentSyntax.HasSingleIdentifier &&
                               symbol.Identities.Any(identity => identity == argument.ArgumentSyntax.Identifiers[0]))
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

        return InternalArguments
            .Take(maxCount)
            .ToArray();
    }

    /// <inheritdoc />
    public IEnumerator<SemanticArgument> GetEnumerator() => InternalArguments.GetEnumerator();

    private IEnumerable<SemanticArgument> InternalArguments => _expandedArguments
        .Where(arg => _flags[arg.OrdinalPosition]);

    private IReadOnlyList<SemanticArgument> ExpandArgumentSyntax(IReadOnlyList<SymbolSyntax> argumentSyntax)
    {
        var terminated = false;
        var position = 0;
        var list = new List<SemanticArgument>(argumentSyntax.Count * 3);

        for (var c = 0; c < argumentSyntax.Count; c++)
        {
            var syntax = argumentSyntax[c];
            var next = c < argumentSyntax.Count - 1 ? argumentSyntax[c + 1] : null;

            switch (syntax)
            {
                case not null when terminated:
                    list.Add(new SemanticArgument(RemoveArgument, syntax, position++));
                    break;
                
                case { Type: SymbolSyntaxType.ArgumentTerminator }:
                    terminated = true;
                    break;
                
                case { Type: SymbolSyntaxType.PosixPrefixed, Identifiers.Length: > 1 }:
                    list.AddRange(syntax.Identifiers.SkipLast(1).Select(identifier => new SemanticArgument(
                        RemoveArgument, 
                        SymbolSyntax.Parse(identifier), 
                        position++)));
                    list.Add(new SemanticArgument(
                        RemoveArgument, 
                        SymbolSyntax.Parse($"{syntax.Identifiers.Last()}{syntax.OperandExpression}"),
                        position++));
                    break;
                
                case { IsPrefixed: true, HasOperand: true }:
                    list.Add(new SemanticArgument(RemoveArgument, syntax, position++));
                    break;
                
                case { IsPrefixed: true } when next is { Type: not SymbolSyntaxType.ArgumentTerminator }:
                    list.Add(new SemanticArgument(RemoveArgument, syntax, position++, next, position + 1));
                    break;
                
                default:
                    list.Add(new SemanticArgument(RemoveArgument, syntax!, position++));
                    break;
            }   
        }

        return list;
    }

    /// <inheritdoc />
    public override string ToString() => $"({this.Count()})";

    private void RemoveArgument(int index) => _flags[index] = false;

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}