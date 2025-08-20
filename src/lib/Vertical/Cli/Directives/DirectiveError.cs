using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Directives;

/// <summary>
/// Represents an error in the usage of a directive.
/// </summary>
public sealed class DirectiveError : UsageError
{
    internal DirectiveError(Token token, string message)
    {
        Token = token;
        Message = message;
    }

    /// <summary>
    /// Gets the directive token.
    /// </summary>
    public Token Token { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; }

    /// <inheritdoc />
    public override void WriteMessages(TextWriter textWriter)
    {
        textWriter.WriteLine($"Invalid directive usage '{Token}': {Message}");
    }
}