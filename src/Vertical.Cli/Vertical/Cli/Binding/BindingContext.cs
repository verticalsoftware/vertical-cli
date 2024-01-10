namespace Vertical.Cli.Binding;

/// <summary>
/// Creates a binding context.
/// </summary>
public static class BindingContext
{
    /// <summary>
    /// Creates a binding context.
    /// </summary>
    /// <param name="rootCommand">The root command definition.</param>
    /// <param name="args">The arguments supplied by the command line.</param>
    /// <param name="defaultValue">The default value to return for help or exception call sites.</param>
    /// <typeparam name="TModel">Model type.</typeparam>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <returns>A context that describes the result of argument parsing and binding.</returns>
    public static IBindingContext<TResult> Create<TModel, TResult>(
        IRootCommand<TModel, TResult> rootCommand,
        IEnumerable<string> args,
        TResult defaultValue)
        where TModel : class
    {
        return BindingContextFactory.Create(rootCommand, args, defaultValue);
    }
}