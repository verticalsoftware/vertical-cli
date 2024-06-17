namespace Vertical.Cli.SourceGenerator;

public sealed class Separator
{
    public Separator(SeparatorStyle style)
    {
        Style = style;
        Iteration = -1;
    }

    public SeparatorStyle Style { get; }
    
    public int Iteration { get; private set; }

    public void Next() => ++Iteration;
}