namespace Vertical.Cli.Help;

/// <summary>
/// Contains keys needed to locate help content.
/// </summary>
public sealed class HelpTopicKey
{
    internal HelpTopicKey(string typeId, string topicId)
    {
        TypeId = typeId;
        TopicId = topicId;
    }

    /// <summary>
    /// Gets the type id.
    /// </summary>
    public string TypeId { get; }

    /// <summary>
    /// Gets the topic id.
    /// </summary>
    public string TopicId { get; }

    /// <inheritdoc />
    public override string ToString() => $"{TypeId} = '{TopicId}'";
}