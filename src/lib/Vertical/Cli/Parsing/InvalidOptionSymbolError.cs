using Vertical.Cli.Invocation;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents a symbol that was used in the CLI arguments but not defined by the application.
/// </summary>
public sealed class InvalidOptionSymbolError : UsageError
{
    internal InvalidOptionSymbolError(Token token)
    {
        Token = token;
    }

    /// <summary>
    /// Gets the invalid symbol token.
    /// </summary>
    public Token Token { get; }

    /// <inheritdoc />
    public override void WriteMessages(TextWriter textWriter)
    {
        textWriter.WriteLine($"Invalid option or switch '{Token}'");
    }
}