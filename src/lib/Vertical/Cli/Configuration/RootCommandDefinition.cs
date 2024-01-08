namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the root command.
/// </summary>
/// <typeparam name="TModel">The type of model used by the handler implementation.</typeparam>
/// <typeparam name="TResult">The type of result produced by the command handler.</typeparam>
internal sealed class RootCommandDefinition<TModel, TResult> :
    CommandDefinition<TModel, TResult>,
    IRootCommand<TModel, TResult>
    where TModel : class
{
    internal RootCommandDefinition(ICommandDefinition<TModel, TResult> definition) : base(definition)
    {
    }
}