using Vertical.Cli.Configuration;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Invocation;

internal sealed class MiddlewareHandler(
    Middleware middleware,
    Guid? registrationId = null,
    Action<IContextBuilder>? appendSymbols = null)
{
    public Guid RegistrationId { get; } = registrationId ?? Guid.NewGuid();

    public Middleware Action => middleware;

    public void AppendSymbols(IContextBuilder builder)
    {
        appendSymbols?.Invoke(builder);
    }
}