namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines a delegate that handles the application's logic.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
public delegate Task<int> CommandHandler<in TModel>(TModel options, CancellationToken cancellation);