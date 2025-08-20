using Vertical.Cli.Invocation;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents a token that was not matched by the contextual parsing operation.
/// </summary>
public sealed class ExtraneousArgumentError : UsageError
{
    internal ExtraneousArgumentError(Token token)
    {
        Token = token;
    }

    /// <summary>
    /// Gets the extraneous token.
    /// </summary>
    public Token Token { get; }

    /// <inheritdoc />
    public override void WriteMessages(TextWriter textWriter)
    {
        textWriter.WriteLine($"Unrecognized argument '{Token}'");
    }
}