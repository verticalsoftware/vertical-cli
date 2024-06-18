using System.Text;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// Options used by <see cref="DefaultHelpProvider"/>
/// </summary>
public sealed class DefaultHelpOptions
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
    /// Gets a function that computes the desired render width.
    /// </summary>
    public Func<int> RenderWidth { get; init; } = () => Console.WindowWidth;

    /// <summary>
    /// Gets a comparer used to sort commands and symbols.
    /// </summary>
    public IComparer<CliObject> NameComparer { get; init; } = IdentifierComparer.Default;

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
    
    /// <summary>
    /// Gets whether to double-space lists.
    /// </summary>
    public bool DoubleSpace { get; init; }
}