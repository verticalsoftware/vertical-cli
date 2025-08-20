namespace Vertical.Cli.SourceGenerator;

public ref struct Separator
{
    public Separator(string next, string? initial = null)
    {
        _next = next;
        _current = _initial = initial ?? string.Empty;
    }

    private readonly string _initial;
    private readonly string _next;
    private string _current;

    public string Next
    {
        get
        {
            var result = _current;
            _current = _next;
            return result;
        }
    }

    public void Reset() => _current = _initial;
}