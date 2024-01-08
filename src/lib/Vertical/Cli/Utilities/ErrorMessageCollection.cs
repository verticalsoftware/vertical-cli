using System.Collections;

namespace Vertical.Cli.Utilities;

internal sealed class ErrorMessageCollection : IEnumerable<string>
{
    private readonly MessageBuilder _messageBuilder = new();
    private readonly List<string> _errors = new();

    internal void Add(Action<MessageBuilder> action)
    {
        _messageBuilder.Clear();
        _messageBuilder.Append($"({_errors.Count + 1}) ");
        action(_messageBuilder);
        _errors.Add(_messageBuilder.ToString());
    }
    
    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator() => _errors.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}