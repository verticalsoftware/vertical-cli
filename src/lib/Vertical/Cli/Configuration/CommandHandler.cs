namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines a delegate that handles the application's logic.
/// </summary>
public delegate Task<int> CommandHandler(CancellationToken cancellationToken);

/// <summary>
/// Defines a delegate that handles the application's logic using bound arguments.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
public delegate Task<int> CommandHandler<in TModel>(TModel options, CancellationToken cancellation);