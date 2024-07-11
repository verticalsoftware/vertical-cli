namespace Vertical.Cli.Parsing;

/// <summary>
/// Defines a delegate used to pre-process arguments.
/// </summary>
/// <param name="argumentList">A mutable argument list.</param>
/// <param name="next">An action that invokes the next pre-processor.</param>
public delegate void ArgumentPreProcessor(LinkedList<string> argumentList,
    Action<LinkedList<string>> next);