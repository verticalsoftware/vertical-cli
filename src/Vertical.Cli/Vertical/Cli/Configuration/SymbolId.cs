namespace Vertical.Cli.Configuration;

internal sealed class SymbolId
{
    internal const int Root = -1;

    private int _next = Root;

    public int Next() => ++_next;
}