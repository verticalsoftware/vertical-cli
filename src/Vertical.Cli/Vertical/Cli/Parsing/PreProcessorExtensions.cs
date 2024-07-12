namespace Vertical.Cli.Parsing;

internal static class PreProcessorExtensions
{
    internal static void EvaluateEach(
        this LinkedList<string> argumentList,
        Func<string, string> replacementFunction)
    {
        for (var node = argumentList.First; node != null; node = node.Next)
        {
            var value = replacementFunction(node.Value);

            if (value.Equals(node.Value))
                continue;

            var thisNode = node;
            node = argumentList.AddAfter(node, value);
            argumentList.Remove(thisNode);
        }
    }
}