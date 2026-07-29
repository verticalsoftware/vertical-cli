using Vertical.Cli.Configuration;
using Vertical.Cli.IO;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Represents an error detected from application input.
/// </summary>
public abstract class CommandLineError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLineError"/> class.
    /// </summary>
    /// <param name="message"></param>
    protected CommandLineError(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; }

    /// <inheritdoc />
    public override string ToString() => Message;

    /// <summary>
    /// Writes the message to the output abstraction.
    /// </summary>
    /// <param name="writer">The output writer.</param>
    public virtual void WriteOutputMessage(OutputWriter writer)
    {
        writer.SetDisplayElement(DisplayElement.Important);
        writer.WriteLine(Message);
    }

    /// <summary>
    /// Gets a description of the symbol for output.
    /// </summary>
    /// <param name="symbol">The symbol instance.</param>
    /// <returns><see cref="string"/></returns>
    public static string GetSymbolIdentifier(ICliSymbol symbol)
    {
        return symbol switch
        {
            CliSymbol { SymbolKind: SymbolKind.PositionArgument, HelpTopic.ParameterSyntax: { } parameterSyntax }
                => $"Argument {parameterSyntax}",
            
            CliSymbol { SymbolKind: SymbolKind.PositionArgument } argumentSymbol 
                => $"Argument {argumentSymbol.BindingName.ToKebabCase(toUpperCase: true)}",
            
            CliSymbol { SymbolKind: SymbolKind.Option } optionSymbol => $"Option {FormatAliases(optionSymbol.Aliases)}",
            
            CliSymbol { SymbolKind: SymbolKind.Switch } switchSymbol => $"Switch {FormatAliases(switchSymbol.Aliases)}",
            
            DirectiveSymbol directiveSymbol => $"Directive [{directiveSymbol.Symbol}]",
            
            _ => throw new NotSupportedException()
        };
    }

    private static string FormatAliases(string[] aliases) => string.Join(", ", aliases);
}