namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines a model with no properties, useful for commands that don't require parameters.
/// </summary>
public sealed class EmptyModel
{
    private EmptyModel()
    {
        
    }

    internal static readonly EmptyModel Instance = new();
}