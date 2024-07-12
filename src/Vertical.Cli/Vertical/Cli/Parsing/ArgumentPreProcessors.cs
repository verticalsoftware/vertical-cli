namespace Vertical.Cli.Parsing;

/// <summary>
/// Defines out-of-box argument pre-processors.
/// </summary>
public static class ArgumentPreProcessors
{
    /// <summary>
    /// Replaces tokens in the arguments with environment variable values.
    /// </summary>
    /// <param name="argumentList">Argument list</param>
    public static void ReplaceEnvironmentVariables(LinkedList<string> argumentList)
    {
        EnvironmentVariablePreProcessor.Handle(argumentList);
    }

    /// <summary>
    /// Replaces tokens in the arguments with environment variable values and invokes the next
    /// processor.
    /// </summary>
    /// <param name="argumentList">Argument list</param>
    /// <param name="next">Action that invokes the next pre-processor</param>
    public static void ReplaceEnvironmentVariables(LinkedList<string> argumentList, Action<LinkedList<string>> next)
    {
        ReplaceEnvironmentVariables(argumentList);
        next(argumentList);
    }

    /// <summary>
    /// Replaces tokens in the arguments with special folder paths.
    /// </summary>
    /// <param name="argumentList">Argument list.</param>
    public static void ReplaceSpecialFolderPaths(LinkedList<string> argumentList)
    {
        SpecialFolderPreProcessor.Handle(argumentList);
    }
    
    /// <summary>
    /// Replaces tokens in the arguments with special folder paths.
    /// </summary>
    /// <param name="argumentList">Argument list.</param>
    /// <param name="next">Action that invokes the next pre-processor</param>
    public static void ReplaceSpecialFolderPaths(LinkedList<string> argumentList, Action<LinkedList<string>> next)
    {
        SpecialFolderPreProcessor.Handle(argumentList);
        next(argumentList);
    }

    /// <summary>
    /// Performs pre-processing using the default stream provider.
    /// </summary>
    /// <param name="argumentList">The mutable argument list.</param>
    public static void AddResponseFileArguments(LinkedList<string> argumentList)
    {
        ResponseFilePreProcessor.Handle(argumentList, fileInfo => File.OpenRead(fileInfo.FullName));
    }

    /// <summary>
    /// Performs pre-processing using the default stream provider, and invokes the next
    /// pre-processor.
    /// </summary>
    /// <param name="argumentList">The mutable argument list.</param>
    /// <param name="next">An action that invokes the next pre-processor.</param>
    public static void AddResponseFileArguments(LinkedList<string> argumentList, Action<LinkedList<string>> next)
    {
        AddResponseFileArguments(argumentList);
        next(argumentList);
    }
}