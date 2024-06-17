namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a sub-command of a an application.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
/// <typeparam name="TResult">Result type</typeparam>
public class SubCommand<TModel, TResult> : CliCommand<TModel, TResult> where TModel : class
{
    /// <inheritdoc />
    public SubCommand(string name, string? description = null) : base([name], description, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubCommand{TModel,TResult}"/> type.
    /// </summary>
    /// <param name="names">Names the command are known by.</param>
    /// <param name="description">Optional description.</param>
    public SubCommand(string[] names, string? description = null) : base(names, description, null)
    {
    }
}