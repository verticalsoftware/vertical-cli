namespace Vertical.Cli.Parsing;

/// <summary>
/// Responsible for pre-processing arguments.
/// </summary>
public static class ArgumentPreProcessorPipeline
{
    /// <summary>
    /// Invokes pre-processing on the given arguments.
    /// </summary>
    /// <param name="arguments">Arguments to pre-process.</param>
    /// <param name="preProcessors">A list of pre-processors.</param>
    /// <returns>The mutated argument list.</returns>
    public static IReadOnlyCollection<string> Invoke(
        IEnumerable<string> arguments,
        IEnumerable<ArgumentPreProcessor> preProcessors)
    {
        var last = new Action<LinkedList<string>>(_ => { });
        var pipeline = preProcessors
            .Reverse()
            .Aggregate(last, (next, processor) => list => processor(list, next));

        var argumentList = new LinkedList<string>(arguments);

        pipeline(argumentList);

        return argumentList;
    }
}