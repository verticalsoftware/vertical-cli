namespace Vertical.Cli.Invocation;

/// <summary>
/// Represents the implementation of a step in a chain-of-responsibility pattern.
/// </summary>
public delegate Task Middleware(InvocationContext context, Func<InvocationContext, Task> next);

/// <summary>
/// Middlware extensions
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Aggregates all steps in the given pipeline into a single delegate.
    /// </summary>
    /// <param name="pipeline">The pipeline of middleware components.</param>
    /// <typeparam name="TContext">The context object available to each middleware.</typeparam>
    public static Middleware Build<TContext>(this IEnumerable<Middleware> pipeline)
    {
        return pipeline.Aggregate((first, second) => (context, next) => first(context, ctx => second(ctx, next)));
    }
    
    /// <summary>
    /// Aggregates all steps in the given pipeline into a single delegate.
    /// </summary>
    /// <param name="pipeline">The pipeline of middleware components.</param>
    public static Middleware Build(
        this IEnumerable<Middleware> pipeline)
    {
        var middleware = pipeline.ToArray();

        return middleware.Length == 0
            ? (_, _) => Task.CompletedTask
            : pipeline
                .Aggregate((first, second) =>
                    (context, next) => 
                        first(context, ctx => second(ctx, next)));
    }
}