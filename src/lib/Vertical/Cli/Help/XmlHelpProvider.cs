using System.Xml.XPath;
using Vertical.Cli.Configuration;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

/// <summary>
/// Implements a <see cref="IHelpProvider"/> using an xml resource.
/// </summary>
public sealed class XmlHelpProvider : IHelpProvider
{
    private record Prefix(string Key, string Value);

    private readonly Lazy<(XPathDocument Document, XPathNavigator Navigator)> _lazyResources;
    private readonly Lazy<Prefix[]> _lazyIdPrefixes;
    private readonly bool _strictContent;

    private const string ParameterNameAttribute = "parameter-name";

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlHelpProvider"/> class.
    /// </summary>
    /// <param name="resourceStreamProvider">
    /// A function invoked by the help system that returns the xml content stream.
    /// </param>
    /// <param name="strictContent"><c>true</c> to throw exceptions when any help element cannot be found.</param>
    public XmlHelpProvider(Func<Stream> resourceStreamProvider, bool strictContent = false)
    {
        _strictContent = strictContent;
        _lazyResources = new Lazy<(XPathDocument Document, XPathNavigator Navigator)>(() =>
        {
            var document = new XPathDocument(resourceStreamProvider());
            var navigator = document.CreateNavigator();
            return (document, navigator);
        });

        _lazyIdPrefixes = new Lazy<Prefix[]>(() =>
        {
            return Navigator
                .Select("/help/prefixes/prefix")
                .Cast<XPathNavigator>()
                .Select(node => new Prefix(
                    $"({node.GetAttribute("key", string.Empty)})",
                    node.GetAttribute("value", string.Empty)
                ))
                .Where(t => t is { Key.Length: > 2, Value.Length: > 0 })
                .ToArray();
        });
    }

    private XPathNavigator Navigator => _lazyResources.Value.Navigator;

    /// <inheritdoc />
    public string? GetRemarks(IHelpSubject subject)
    {
        return subject switch
        {
            Command command => GetNode(command)?.SelectSingleNode("remarks")?.Value,
            CliSymbol symbol => GetNode(symbol)?.Value,
            _ => GetNode(subject)?.Value
        } ?? subject.GetRemarks();
    }

    /// <inheritdoc />
    public IEnumerable<ExtendedRemarksSection> GetExtendedRemarks(Command command)
    {
        var result = GetResult().ToArray();
        return result.Length > 0 ? result : command.GetExtendedRemarksSections();
        
        IEnumerable<ExtendedRemarksSection> GetResult()
        {
            var sectionNodesIterator = GetNode(command)?
                .SelectSingleNode("sections")?
                .SelectChildren(XPathNodeType.Element);

            if (sectionNodesIterator is null)
                yield break;

            while (sectionNodesIterator.MoveNext())
            {
                var current = sectionNodesIterator.Current;
                if (current is null)
                    continue;

                var title = current.GetAttribute("title", string.Empty);
                var remarks = current.Value;

                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(remarks))
                    continue;

                yield return new ExtendedRemarksSection(title, remarks);
            }
        }
    }

    /// <inheritdoc />
    public string GetIdentifier(IHelpSubject subject)
    {
        return subject is CliSymbol { Kind: SymbolKind.PositionArgument } symbol
               && GetAttribute(symbol, ParameterNameAttribute) is { } identifier
            ? identifier
            : subject.GetListIdentifier();
    }

    /// <inheritdoc />
    public string? GetParameterName(ICliSymbol subject)
    {
        return subject is { Kind: SymbolKind.Option or SymbolKind.Directive }
               && GetAttribute(subject, ParameterNameAttribute) is { } parameterName
            ? parameterName
            : subject.GetParameterName();
    }

    private XPathNavigator? GetNode(IHelpSubject subject)
    {
        var key =  subject.HelpTopicKey;
        var id = CreateLookupId(key.TopicId);
        var path = $"/help/topic[@type='{key.TypeId}' and @id='{id}']";

        if (Navigator.SelectSingleNode(path) is { } node)
            return node;

        if (!_strictContent) return null;

        throw new InvalidOperationException(
            $"""
             Failed to resolve element in Xml help document (strict resolution configured).
               type:        {key.TypeId}
               topicId:     {key.TopicId}
               lookup:      {id}
               x-path:      {path}
             """);
    }

    private string CreateLookupId(string topicId)
    {
        foreach (var prefix in _lazyIdPrefixes.Value)
        {
            if (!topicId.StartsWith(prefix.Value))
                continue;

            return $"{prefix.Key}{topicId[prefix.Value.Length..]}";
        }

        return topicId;
    }

    private string? GetAttribute(IHelpSubject subject, string attributeName)
    {
        var node = GetNode(subject);
        var attribute = node?.GetAttribute(attributeName, namespaceURI: string.Empty);
        return attribute.NonWhiteSpaceOrNull();
    }
}