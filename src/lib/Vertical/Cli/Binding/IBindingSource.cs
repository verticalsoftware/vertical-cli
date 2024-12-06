namespace Vertical.Cli.Binding;

/// <summary>
/// Represents a binding source.
/// </summary>
public interface IBindingSource
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    bool TryGetValue(out object? obj);
}