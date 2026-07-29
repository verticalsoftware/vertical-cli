using System.Linq.Expressions;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents the syntax of an argument.
/// </summary>
public sealed partial class ArgumentSyntax
{
    internal ArgumentSyntax(
        string text,
        SyntaxKind syntaxKind,
        string? symbol = null,
        char? separatorToken = null,
        string? parameterValue = null)
    {
        Text = text;
        SyntaxKind = syntaxKind;
        Symbol = symbol;
        SeparatorToken = separatorToken;
        ParameterValue = parameterValue;
    }

    /// <summary>
    /// Gets the original argument text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the syntax class.
    /// </summary>
    public SyntaxKind SyntaxKind { get; }

    /// <summary>
    /// Gets the option identifier, or <c>null</c> if the text is not an option.
    /// </summary>
    public string? Symbol { get; }

    /// <summary>
    /// Gets the token that separates the identifier and parameter value.
    /// </summary>
    public char? SeparatorToken { get; }

    /// <summary>
    /// Gets the attached option parameter value.
    /// </summary>
    public string? ParameterValue { get; }

    /// <inheritdoc />
    public override string ToString() => $"{SyntaxKind} '{Text}'";

    /// <summary>
    /// Creates a gnu option for the given property.
    /// </summary>
    /// <param name="expression">Expression where the property name if derived.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A GNU long option alias.</returns>
    public static string CreateGnuAlias<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
    {
        return CreateGnuAlias(expression.BindingName);
    }

    internal static string CreateGnuAlias(string bindingName)
    {
        return bindingName.ToKebabCase("--");
    }
}