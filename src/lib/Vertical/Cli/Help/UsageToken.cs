namespace Vertical.Cli.Help;

/// <summary>
/// Defines part of a usage statement.
/// </summary>
/// <param name="ElementKind">Element kind</param>
/// <param name="Text">The token</param>
public readonly record struct UsageToken(HelpElementKind ElementKind, string Text);