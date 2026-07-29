using Vertical.Cli.Invocation;

namespace Vertical.Cli.Middleware;

/// <summary>
/// Defines a middleware delegate.
/// </summary>
public delegate Task MiddlewareDelegate(InvocationContext context, Func<InvocationContext, Task> next);