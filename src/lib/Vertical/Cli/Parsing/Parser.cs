using Vertical.Cli.Internal;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents the default implementation of a parser.
/// </summary>
public sealed class Parser : IParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Parser"/> class.
    /// </summary>
    /// <param name="options">The options used to control the parser's behavior.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null</exception>
    public Parser(IParserOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Gets the parser's options.
    /// </summary>
    public IParserOptions Options { get; }

    /// <inheritdoc />
    public bool IsTerminatorToken(string value) => TokenSyntax.Parse(value).Kind == SyntaxKind.Terminator;

    /// <inheritdoc />
    public string CreateSymbol(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        return Options.DefaultSymbolConvention switch
        {
            SymbolConvention.GnuOption => name.ToKebabCase(['-', '-']),
            SymbolConvention.PosixOption => $"-{name[0]}",
            _ => name.ToCamelCase(['/'])
        };
    }

    /// <inheritdoc />
    public IEnumerable<Token> ParseArguments(IEnumerable<string> arguments)
    {
        var terminated = false;
        var directivesTerminated = false;
        var index = 0;

        foreach (var argument in arguments)
        {
            var syntax = TokenSyntax.Parse(argument);

            directivesTerminated = directivesTerminated || syntax.Kind != SyntaxKind.EnclosedSymbol;

            switch (terminated, syntax)
            {
                case { terminated: true }:
                    yield return new Token(TokenKind.TerminatedValue, syntax, index++);
                    break;
                
                case { syntax.Kind: SyntaxKind.Terminator }:
                    yield return new Token(TokenKind.Terminator, syntax, index++);
                    terminated = true;
                    continue;
                
                case { syntax.Kind: SyntaxKind.EnclosedSymbol } when !directivesTerminated:
                    yield return new Token(TokenKind.Directive, syntax, -1);
                    continue;
                
                case { syntax.SymbolConvention: SymbolConvention.GnuOption }:
                case { syntax.SymbolConvention: SymbolConvention.PosixGroup } when Options.ParsePosixGroups:
                case { syntax.SymbolConvention: SymbolConvention.PosixOption }:
                case { syntax.SymbolConvention: SymbolConvention.ForwardSlashOption } when Options.ParseWindowsStyleOptions:
                    yield return new Token(TokenKind.OptionSymbol, syntax, index++);
                    break;

                default:
                    yield return new Token(TokenKind.ArgumentValue, syntax, index++);
                    break;
            }
        }
    }

    /// <inheritdoc />
    public SemanticToken[] CreateSemanticTokens(Token token)
    {
        return token is { Kind: TokenKind.OptionSymbol, Syntax.SymbolConvention: SymbolConvention.PosixGroup }
            ? CreatePosixGroupSemanticToken(token)
            : [new SemanticToken(token, token)];
    }

    private static SemanticToken[] CreatePosixGroupSemanticToken(Token token)
    {
        var symbolSpan = token.Syntax.NonPrefixedSymbolSpan;
        var symbolCount = symbolSpan.Length;
        var tokens = new SemanticToken[symbolCount];

        for (var c = 0; c < symbolCount - 1; c++)
        {
            var switchSyntax = TokenSyntax.Parse($"-{symbolSpan[c]}");
            
            tokens[c] = new SemanticToken(
                new Token(TokenKind.OptionSymbol, switchSyntax, token.ParsePosition),
                token);
        }

        var optionSyntax = TokenSyntax.Parse($"-{symbolSpan[^1]}{token.Syntax.ParameterSyntaxSpan}");
        tokens[^1] = new SemanticToken(new Token(TokenKind.OptionSymbol, optionSyntax, token.ParsePosition), token);

        return tokens;
    }
}