namespace Vertical.Cli.Binding;

/// <summary>
/// Base model binder type.
/// </summary>
public abstract class ModelBinder
{
    /// <summary>
    /// Gets the model type.
    /// </summary>
    public abstract Type ModelType { get; }
}