using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Directives;

/// <summary>
/// Represents a context that can be used to handle a directive.
/// </summary>
public class DirectiveContext
{
    private readonly InvocationContext _context;

    internal DirectiveContext(Token token, InvocationContext context)
    {
        _context = context;
        Token = token;
    }


    internal bool Dequeue { get; private set; }

    /// <summary>
    /// Gets the directive token.
    /// </summary>
    public Token Token { get; }

    /// <summary>
    /// Removes the token from the internal list, removing its visibility for any subsequent
    /// middleware components.
    /// </summary>
    public void DequeueToken()
    {
        Dequeue = true;
    }

    /// <summary>
    /// Adds an application defined error indicating improper usage.
    /// </summary>
    /// <param name="message">The message to display with the token's value.</param>
    public void AddError(string message)
    {
        _context.Errors.Add(new DirectiveError(Token, message));
    }

    /// <summary>
    /// Sets the exit code, which will stop the application.
    /// </summary>
    /// <param name="code">The code to return.</param>
    public void SetExitCode(int code)
    {
        _context.ExitCode = code;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Token}, dequeue={Dequeue}";
}

/// <summary>
/// Represents a context that can be used to handle a directive.
/// </summary>
/// <typeparam name="TState">The state object type.</typeparam>
public sealed class DirectiveContext<TState> : DirectiveContext
{
    /// <inheritdoc />
    internal DirectiveContext(
        Token token, 
        TState state,
        InvocationContext context) : base(token, context)
    {
        State = state;
    }

    /// <summary>
    /// Gets the application defined context object.
    /// </summary>
    public TState State { get; }
}