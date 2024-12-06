namespace Vertical.Cli.Routing;

/// <summary>
/// Defines a call site delegate that does not accept a model.
/// </summary>
public delegate int CallSite();

/// <summary>
/// Defines an async call site delegate that accepts a model.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
public delegate int CallSite<in TModel>(TModel model);

/// <summary>
/// Defines an async call site delegate that does not accept a model.
/// </summary>
public delegate Task<int> AsyncCallSite(CancellationToken cancellationToken);

/// <summary>
/// Defines an async call site delegate that accepts a model.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
public delegate Task<int> AsyncCallSite<in TModel>(TModel model, CancellationToken cancellationToken)
    where TModel : class;