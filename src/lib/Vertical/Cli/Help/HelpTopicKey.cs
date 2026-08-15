namespace Vertical.Cli.Help;

/// <summary>
/// Contains keys needed to locate help content.
/// </summary>
public sealed class HelpTopicKey
{
    internal HelpTopicKey(string typeId, string topic)
    {
        TypeId = typeId;
        Topic = topic;
    }

    /// <summary>
    /// Gets the type id.
    /// </summary>
    public string TypeId { get; }

    /// <summary>
    /// Gets the topic id.
    /// </summary>
    public string Topic { get; }

    /// <inheritdoc />
    public override string ToString() => $"{TypeId} = '{Topic}'";
}