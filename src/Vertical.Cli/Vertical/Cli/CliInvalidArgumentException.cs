using Vertical.Cli.Parsing;

namespace Vertical.Cli;

/// <summary>
/// Indicates the number of provided values for an argument does not satisfy
/// the arity definition.
/// </summary>
public sealed class CliInvalidArgumentException : CliArgumentException
{
    /// <inheritdoc />
    internal CliInvalidArgumentException(
        string format,
        IDictionary<string, object> arguments,
        IReadOnlyCollection<SemanticArgument> invalidArguments,
        Exception? innerException = null) 
        : base(format, arguments, innerException)
    {
        InvalidArguments = invalidArguments;
    }

    /// <summary>
    /// Gets the invalid arguments.
    /// </summary>
    public IReadOnlyCollection<SemanticArgument> InvalidArguments { get; }
}