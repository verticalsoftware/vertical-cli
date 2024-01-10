namespace Vertical.Cli.Configuration;


/// <summary>
/// Represents the definition of a CLI command.
/// </summary>
/// <typeparam name="TModel">The type of model used by the handler implementation.</typeparam>
/// <typeparam name="TResult">The type of result produced by the command handler.</typeparam>
public interface ICommandDefinition<in TModel, TResult> : ICommandDefinition<TResult>
    where TModel : class
{
    /// <summary>
    /// Gets the function that implements the application's logic for this command.
    /// </summary>
    Func<TModel, CancellationToken, TResult>? Handler { get; }
}