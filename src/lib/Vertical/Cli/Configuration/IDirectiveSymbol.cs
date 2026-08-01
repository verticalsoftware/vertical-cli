using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration;

public interface IDirectiveSymbol : ICliSymbol
{
    /// <summary>
    /// Gets the directive identifier.
    /// </summary>
    string Identifier { get; }

    /// <summary>
    /// Gets the parameter arity.
    /// </summary>
    ParameterArity ParameterArity { get; }

    /// <summary>
    /// Asynchronously handles the matched token action.
    /// </summary>
    /// <param name="context">Invocation context.</param>
    /// <param name="token">Token matched from user input.</param>
    /// <returns>Task</returns>
    Task HandleAsync(InvocationContext context, ArgumentToken token);
}