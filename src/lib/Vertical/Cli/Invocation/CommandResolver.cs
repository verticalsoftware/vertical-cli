using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Determines the target command.
/// </summary>
public static class CommandResolver
{
    /// <summary>
    /// Determines the command target by matching the leading tokens in the provided list
    /// to a command path.
    /// </summary>
    /// <param name="rootCommand">The application's root command.</param>
    /// <param name="tokenList">The token list.</param>
    /// <returns>
    /// A tuple containing the matched command and the last token that matched the command
    /// path (or null if the root command is the target). 
    /// </returns>
    public static (Command Target, ArgumentToken? EvaluatedToken) GetTarget(
        RootCommand rootCommand,
        TokenList tokenList)
    {
        Command command = rootCommand;
        ArgumentToken? evaluatedToken = null;
        var token = tokenList.First;

        while (token is { Kind: TokenKind.CommandOrArgument })
        {
            var child = command
                .SubCommands
                .FirstOrDefault(cmd => cmd.Name.Equals(token.Text));

            if (child is null)
                break;

            command = child;
            evaluatedToken = token; 
            token = token.Next;                
        }

        return (command, evaluatedToken);
    }
}