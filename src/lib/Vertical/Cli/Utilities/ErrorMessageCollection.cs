using System.Collections;

namespace Vertical.Cli.Utilities;

internal sealed class ErrorMessageCollection : IEnumerable<string>
{
    private readonly TextBuilder _textBuilder = new();
    private readonly List<string> _errors = new();

    internal void Add(Action<TextBuilder> action)
    {
        _textBuilder.Clear();
        _textBuilder.Append($"({_errors.Count + 1}) ");
        action(_textBuilder);
        _errors.Add(_textBuilder.ToString());
    }
    
    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator() => _errors.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}