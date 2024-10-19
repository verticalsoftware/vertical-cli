using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Transforms arguments.
/// </summary>
public delegate List<ArgumentSyntax> ArgumentTransformer(List<ArgumentSyntax> syntaxList);