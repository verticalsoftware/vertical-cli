using System.Text;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Options used by <see cref="DefaultHelpProvider"/>
/// </summary>
public sealed class DefaultHelpProviderOptions
{
    /// <summary>
    /// Gets the help content provider.
    /// </summary>
    public IHelpContentProvider ContentProvider { get; init; } = new DefaultHelpContentProvider();

    /// <summary>
    /// Gets the number of spaces for each indent operation.
    /// </summary>
    public int IndentSpaces { get; init; } = 3;

    /// <summary>
    /// Gets the desired render width.
    /// </summary>
    public int RenderWidth { get; init; }

    /// <summary>
    /// Gets a function that formats operand argument names.
    /// </summary>
    public Action<StringBuilder, CliSymbol> OperandNameFormatter { get; init; } = (sb, symbol) => symbol
        .BindingName
        .Aggregate(char.MinValue, (prev, next) =>
        {
            if (char.IsUpper(next) && char.IsLower(prev))
                sb.Append('_');
            sb.Append(char.ToUpper(next));
            return next;
        });
}