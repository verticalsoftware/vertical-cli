using Vertical.Cli.Parsing;

namespace Vertical.Cli.Invocation;

public sealed class TokenFilterEventInfo
{
    internal TokenFilterEventInfo(InvocationContext context, ArgumentToken token)
    {
        Context = context;
        Token = token;
    }

    /// <summary>
    /// Gets the invocation context.
    /// </summary>
    public InvocationContext Context { get; }

    /// <summary>
    /// Gets the matched token.
    /// </summary>
    public ArgumentToken Token { get; }

    /// <summary>
    /// Gets or sets whether to remove the token.
    /// </summary>
    public bool RemoveToken { get; set; } = true;
}