using System.Collections;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents a managed collection of argument tokens.
/// </summary>
public sealed class TokenList : ITokenList
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenList"/> class.
    /// </summary>
    /// <param name="arguments">The initial argument collection.</param>
    public TokenList(IEnumerable<string>? arguments = null)
    {
        if (arguments == null)
            return;
        
        Append(arguments);
    }
    
    /// <summary>
    /// Gets the current version of the list.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenList"/> class that is a
    /// deep copy of the provided list.
    /// </summary>
    /// <param name="tokens">The tokens whose deep clones are added to the list.</param>
    public TokenList(IEnumerable<ArgumentToken> tokens)
    {
        ArgumentNullException.ThrowIfNull(tokens);
        Clone(tokens);
    }

    /// <summary>
    /// Gets the first token in the list.
    /// </summary>
    public ArgumentToken? First { get; private set; }

    /// <summary>
    /// Gets the last token in the list.
    /// </summary>
    public ArgumentToken? Last { get; private set; }

    /// <summary>
    /// Gets the number of tokens in the list.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Clears the argument list of all tokens.
    /// </summary>
    public void Clear()
    {
        foreach (var token in this)
        {
            DisassociateToken(token);
        }

        First = null;
        Last = null;
        Count = 0;
        ++Version;
    }

    /// <summary>
    /// Inserts one or more arguments at the end of the list.
    /// </summary>
    /// <param name="arguments">The arguments to parse and add as tokens.</param>
    /// <returns></returns>
    public void Append(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        ++Version;
        Insert(Last, arguments);
    }

    /// <summary>
    /// Inserts one or more arguments after the given token.
    /// </summary>
    /// <param name="token">The token where the new arguments will be inserted after.</param>
    /// <param name="arguments">The arguments to parse and add as tokens.</param>
    /// <returns>The last token that was inserted.</returns>
    public ArgumentToken? InsertAfter(ArgumentToken token, IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        ArgumentNullException.ThrowIfNull(token);
        
        CheckOwnership(token);
        ++Version;
        return Insert(token, arguments);
    }

    /// <summary>
    /// Removes a token from the list.
    /// </summary>
    /// <param name="token">The token to remove.</param>
    public void Remove(ArgumentToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        CheckOwnership(token);

        token.Previous?.Next = token.Next;
        token.Next?.Previous = token.Previous;

        if (ReferenceEquals(token, First))
            First = token.Next;
        else if (ReferenceEquals(token, Last))
            Last = token.Previous;
        
        DisassociateToken(token);
        --Count;
        ++Version;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Count}";

    private static void DisassociateToken(ArgumentToken token)
    {
        token.Previous = null;
        token.Next = null;
        token.TokenList = null;
    }

    private ArgumentToken? Insert(ArgumentToken? token, IEnumerable<string> arguments)
    {
        var argumentsArray = arguments.ToArray();

        if (argumentsArray.Length == 0)
            return null;
        
        var (queuedArgumentType, terminatedChain) = GetState(token);
        
        var newTokens = argumentsArray
            .SelectMany(argument => Parse(argument, ref queuedArgumentType, ref terminatedChain))
            .ToArray();

        Link(newTokens);

        // Link
        var (head, tail) = (newTokens[0], newTokens[^1]);
        head.Previous = token;
        tail.Next = token?.Next;
        tail.Next?.Previous = tail;
        head.Previous?.Next = head;
        
        // Adjust pointers & count
        First ??= head;
        Last ??= tail;
        Count += newTokens.Length;
        
        // Adjust after insert
        for (var current = tail.Next; current != null; current = current.Next)
        {
            switch (current, queuedArgumentType, terminatedChain)
            {
                //  Convert tokens to arguments
                case { terminatedChain: true, current.Kind: not TokenKind.Argument }:
                    Replace(ref current, new ArgumentToken(this, TokenKind.Argument, current.Text, current.Text));
                    break;
                
                // Convert CommandOrArgument to Argument after options
                case { queuedArgumentType: TokenKind.Argument, current.Kind: TokenKind.CommandOrArgument }:
                    Replace(ref current, new ArgumentToken(this, TokenKind.Argument, current.Text, current.Value));
                    break;
            }
        }

        return tail;
    }

    private static void Replace(ref ArgumentToken token, ArgumentToken replacement)
    {
        replacement.Previous = token.Previous;
        replacement.Next = token.Next;
        token.Previous?.Next = replacement;
        token.Next?.Previous = replacement;

        token.Previous = token.Next = null;
        token.TokenList = null;

        token = replacement;
    }

    private IEnumerable<ArgumentToken> Parse(
        string argument,
        ref TokenKind queuedArgumentType,
        ref bool terminatedChain)
    {
        var syntax = ArgumentSyntax.Parse(argument);

        switch (syntax, terminatedChain)
        {
            case { terminatedChain: true }:
                return [new ArgumentToken(this, TokenKind.Argument, argument, argument)];
            
            case { syntax.SyntaxKind: SyntaxKind.OptionsTerminator, terminatedChain: false }:
                terminatedChain = true;
                return [new ArgumentToken(this, TokenKind.OptionsTerminator, argument, null)];
                
            case { syntax.SyntaxKind: SyntaxKind.None }:
                return [new ArgumentToken(this, queuedArgumentType, argument, argument)];
            
            case { syntax.SyntaxKind: SyntaxKind.Option }:
                queuedArgumentType = TokenKind.Argument;
                return [new ArgumentToken(this, TokenKind.Option, argument, syntax.ParameterValue, syntax.Symbol)];
            
            case { syntax.SyntaxKind: SyntaxKind.OptionGroup }:
                queuedArgumentType = TokenKind.Argument;
                var switchTokens = syntax
                    .Symbol![1..^1]
                    .Select(switchChar => $"-{switchChar}")
                    .Select(switchSymbol => new ArgumentToken(this, TokenKind.Option, argument, null, switchSymbol));
                var option = $"-{syntax.Symbol[^1]}";
                var optionText = syntax.ParameterValue != null
                    ? $"{option}{syntax.SeparatorToken}{syntax.ParameterValue}"
                    : option;
                var optionToken = new ArgumentToken(this, TokenKind.Option, optionText, syntax.ParameterValue, option);

                return switchTokens.Append(optionToken);
            
            case { syntax.SyntaxKind: SyntaxKind.Directive }:
                return [new ArgumentToken(this, TokenKind.Directive, argument, syntax.ParameterValue, syntax.Symbol)];
            
            case { syntax.SyntaxKind: SyntaxKind.Annotation }:
                return [new ArgumentToken(this, TokenKind.Annotation, argument, syntax.ParameterValue)];
            
            default:
                throw new NotSupportedException($"{syntax.SyntaxKind}");
        }
    }

    private (TokenKind, bool) GetState(ArgumentToken? token)
    {
        var queuedKind = TokenKind.CommandOrArgument;
        var terminated = false;
        
        for (var current = First; current != null; current = current.Next)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (current.Kind)
            {
                case TokenKind.Option:
                    queuedKind = TokenKind.Argument;
                    break;
                
                case TokenKind.OptionsTerminator:
                    terminated = true;
                    break;
            }

            if (ReferenceEquals(current, token))
                break;
        }

        return (queuedKind, terminated);
    }

    private void CheckOwnership(ArgumentToken token)
    {
        if (ReferenceEquals(token.TokenList, this))
            return;

        throw new ArgumentException("Token is not owned by this list instance.");
    }

    private void Clone(IEnumerable<ArgumentToken> sourceTokens)
    {
        var tokens = sourceTokens
            .Select(token => new ArgumentToken(this, token.Kind, token.Text, token.Value, token.Symbol))
            .ToArray();

        if (tokens.Length == 0)
            return;

        Link(tokens);
        Count = tokens.Length;
        (First, Last) = tokens.Length > 0
            ? (tokens[0], tokens[^1])
            : (null, null);
    }

    internal static void Link(ArgumentToken[] tokens)
    {
        if (tokens.Length == 0) return;
        
        _ = tokens
            .Aggregate((first, second) =>
            {
                first.Next = second;
                second.Previous = first;
                return second;
            });
    }

    internal sealed class Enumerator(ArgumentToken? head) : IEnumerator<ArgumentToken>
    {
        private ArgumentToken? _current;
        private bool _beforeFirst = true;

        /// <inheritdoc />
        public bool MoveNext()
        {
            _current = _beforeFirst ? head : _current?.Next;
            _beforeFirst = false;
            return _current != null;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _current = null;
            _beforeFirst = true;
        }

        /// <inheritdoc />
        ArgumentToken IEnumerator<ArgumentToken>.Current => _current ?? throw new InvalidOperationException("Enumerator was moved past end.");

        /// <inheritdoc />
        object? IEnumerator.Current => _current;

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }

    /// <inheritdoc />
    public IEnumerator<ArgumentToken> GetEnumerator() => new Enumerator(First);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}