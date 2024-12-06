namespace Vertical.Cli.Configuration;

internal sealed class EmptyModel
{
    public static readonly EmptyModel Instance = new();
    
    private EmptyModel()
    {
    }
}