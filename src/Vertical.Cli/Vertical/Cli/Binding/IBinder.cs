using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Binds argument values given a symbol definition.
/// </summary>
public interface IBinder
{
    /// <summary>
    /// Creates a binding given path data.
    /// </summary>
    /// <param name="bindingContext">An object that contains data about the current command path.</param>
    /// <param name="symbol">The symbol to bind.</param>
    /// <returns><see cref="ArgumentBinding"/></returns>
    ArgumentBinding CreateBinding(IBindingContext bindingContext, SymbolDefinition symbol);
}