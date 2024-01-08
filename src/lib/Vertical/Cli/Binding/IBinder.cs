using Vertical.Cli.Configuration;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

public interface IBinder
{
    /// <summary>
    /// Creates a binding given path data.
    /// </summary>
    /// <param name="bindingPath">An object that contains data about the current command path.</param>
    /// <param name="symbol">The symbol to bind.</param>
    /// <returns><see cref="ArgumentBinding"/></returns>
    ArgumentBinding CreateBinding(IBindingPath bindingPath, SymbolDefinition symbol);
}