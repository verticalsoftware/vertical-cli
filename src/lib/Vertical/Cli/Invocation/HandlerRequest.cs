namespace Vertical.Cli.Invocation;

/// <summary>
/// Encapsulates all the data required to invoke a command and retrieve the result.
/// </summary>
public sealed class HandlerRequest
{
    internal HandlerRequest(Func<Task<int>> callSite)
    {
        _callSite = callSite;
    }
    
    private readonly Func<Task<int>> _callSite;

    /// <summary>
    /// Invokes the command handler and obtains the result.
    /// </summary>
    /// <returns>The result returned by the handler, or an error code.</returns>
    public Task<int> GetResultAsync() => _callSite();

    /// <summary>
    /// Gets a default instance that returns an error code.
    /// </summary>
    public static HandlerRequest Default => new(() => Task.FromResult(-1));
}