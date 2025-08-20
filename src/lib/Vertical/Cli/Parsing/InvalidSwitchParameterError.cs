using Vertical.Cli.Binding;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents when the client specified a value other than a <c>bool</c> for a switch.
/// </summary>
public sealed class InvalidSwitchParameterError : UsageError
{
    internal InvalidSwitchParameterError(ISymbolBinding symbolBinding, Token token)
    {
        SymbolBinding = symbolBinding;
        Token = token;
    }

    /// <summary>
    /// Gets the affected symbol binding.
    /// </summary>
    public ISymbolBinding SymbolBinding { get; }

    /// <summary>
    /// Gets the token.
    /// </summary>
    public Token Token { get; }

    /// <inheritdoc />
    public override void WriteMessages(TextWriter textWriter)
    {
        textWriter.WriteLine($"{SymbolBinding}: invalid parameter '{Token.Syntax.ParameterSpan}' for switch");
    }
}