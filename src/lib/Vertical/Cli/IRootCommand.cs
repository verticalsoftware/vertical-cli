using Vertical.Cli.Configuration;

namespace Vertical.Cli;

/// <summary>
/// Represents configuration of the top-level application command.
/// </summary>
/// <typeparam name="TModel">The type of model used by the handler implementation.</typeparam>
/// <typeparam name="TResult">The type of result produced by the command handler.</typeparam>
public interface IRootCommand<in TModel, TResult> : ICommandDefinition<TModel, TResult>
    where TModel : class
{
}