using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;
using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents the default implementation of a help provider.
/// </summary>
public sealed class DefaultHelpProvider : IHelpProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultHelpProvider"/> class.
    /// </summary>
    /// <param name="layoutEngine">
    /// The component of the help system that aligns, trims, and wraps content
    /// from the help provider before it is rendered by the help text writer.
    /// </param>
    /// <param name="resourceManager">The resource manager that provides help content.</param>
    /// <exception cref="ArgumentNullException"><paramref name="layoutEngine"/> is null.</exception>
    public DefaultHelpProvider(ILayoutEngine layoutEngine, IHelpResourceManager resourceManager)
    {
        LayoutEngine = layoutEngine ?? throw new ArgumentNullException(nameof(layoutEngine));
        ResourceManager = resourceManager;
    }
    internal static IHelpProvider CreateDefault(IConsole console)
    {
        var helpTextWriter = new ColoredOutputHelpWriter(console.Out);
        var layoutEngine = new CompactLayoutEngine(helpTextWriter, console.DisplayWidth);
        var resourceManager = new HelpTagResourceManager();

        return new DefaultHelpProvider(layoutEngine, resourceManager);
    }

    /// <summary>
    /// Gets the layout engine.
    /// </summary>
    public ILayoutEngine LayoutEngine { get; }

    /// <summary>
    /// Gets the resource manager.
    /// </summary>
    public IHelpResourceManager ResourceManager { get; }

    /// <inheritdoc />
    public Task RenderHelpAsync(HelpModel helpModel)
    {
        WriteCommandDescription(helpModel);
        WriteCommandIntroductoryRemarks(helpModel);
        WriteCommandUsages(helpModel);
        WriteSubCommandList(helpModel);
        WriteArgumentSymbolList(helpModel);
        WriteOptionSymbolList(helpModel);
        WriteDirectiveList(helpModel);
        WriteCommandFinalRemarks(helpModel);
        
        return Task.CompletedTask;
    }

    private void WriteCommandDescription(HelpModel helpModel)
    {
        if (ResourceManager.GetCommandDescription(helpModel.Subject) is not { Length: > 0 } content)
            return;

        LayoutEngine.WriteCommandDescriptionSection(
            ResourceManager.DescriptionSectionTitle,
            content);
        
        LayoutEngine.WriteSectionEnd();
    }

    private void WriteCommandUsages(HelpModel helpModel)
    {
        var command = helpModel.Subject;
        
        LayoutEngine.WriteSectionTitle(ResourceManager.UsageSectionTitle);
        
        WriteSubCommandUsage(command);
        WriteInvocationUsage(helpModel, command);
        WriteOptionUsages(helpModel, command);
        LayoutEngine.WriteSectionEnd();
    }

    private void WriteCommandIntroductoryRemarks(HelpModel helpModel)
    {
        if (ResourceManager.GetCommandIntroductoryRemarks(helpModel.Subject) is not { } remarks)
            return;
        
        LayoutEngine.WriteSectionTitle(remarks.SectionTitle);
        LayoutEngine.WriteParagraph(HelpElementKind.CommandDescription, remarks.Content);
        LayoutEngine.WriteSectionEnd();
    }

    private void WriteCommandFinalRemarks(HelpModel helpModel)
    {
        if (ResourceManager.GetCommandFinalRemarks(helpModel.Subject) is not { } remarks)
            return;
        
        LayoutEngine.WriteSectionTitle(remarks.SectionTitle);
        LayoutEngine.WriteParagraph(HelpElementKind.CommandDescription, remarks.Content);
        LayoutEngine.WriteSectionEnd();
    }

    private void WriteSubCommandList(HelpModel helpModel)
    {
        if (helpModel.Subject.Commands is not { Count: > 1 } commands)
            return;

        WriteList(
            ResourceManager.GetCommandGroupings(commands),
            command => new HelpListItem(command.Name, ResourceManager.GetCommandDescription(command))
        );
    }

    private void WriteArgumentSymbolList(HelpModel helpModel)
    {
        if (helpModel.Symbols.ArgumentSymbols is not { Count: > 0 } arguments)
            return;
        
        WriteList(
            ResourceManager.GetArgumentSymbolGroupings(arguments),
            symbol => new HelpListItem(
                ResourceManager.GetArgumentParameterSyntax(symbol),
                ResourceManager.GetSymbolDescription(symbol)));
    }

    private void WriteOptionSymbolList(HelpModel helpModel)
    {
        var optionSymbols = helpModel
            .Symbols
            .OptionSymbols
            .ToArray();
        
        if (optionSymbols is not { Length: > 0 })
            return;
        
        WriteList(
            ResourceManager.GetOptionSymbolGroupings(optionSymbols),
            symbol => new HelpListItem(
                ResourceManager.GetOptionAliasList(symbol),
                ResourceManager.GetSymbolDescription(symbol),
                ResourceManager.GetOptionParameterSyntax(symbol)));
    }

    private void WriteDirectiveList(HelpModel helpModel)
    {
        var directiveHelpTags = helpModel.Symbols.DirectiveHelpTags;
        
        if (directiveHelpTags.Count == 0)
            return;

        WriteList(directiveHelpTags.GroupBy(
            _ => ResourceManager.DirectiveSectionTitle),
            directive => new HelpListItem(directive.UsageSyntax, directive.Description));
    }

    private void WriteList<T>(IEnumerable<IGrouping<string, T>> itemSource, Func<T, HelpListItem> listItemConverter)
    {
        var groupings = itemSource.ToArray();

        foreach (var grouping in groupings)
        {
            var listItems = grouping
                .Select(listItemConverter)
                .ToArray();
            
            LayoutEngine.WriteSectionTitle(grouping.Key);
            LayoutEngine.WriteListItems(listItems);
            LayoutEngine.WriteSectionEnd();
        }
    }

    private void WriteSubCommandUsage(ICommand command)
    {
        if (command.Commands.Count == 0)
            return;

        LayoutEngine.WriteUsageClause(
            command.Path,
            new UsageToken(
                HelpElementKind.SubCommandNameToken,
                ResourceManager.GetSubCommandUsageToken(command.Commands)),
            new UsageToken(
                HelpElementKind.SubCommandArgumentsAndOptionsToken,
                ResourceManager.SubCommandUsageArgumentsToken));
    }

    private void WriteInvocationUsage(HelpModel helpModel, ICommand command)
    {
        if (command is not IInvocationTarget)
            return;
        
        var symbols = helpModel.GetSymbols(command);
        var (hasArguments, hasOptions) = (symbols.ArgumentSymbols.Count > 0, symbols.OptionSymbols.Count > 0);

        switch (hasArguments, hasOptions)
        {
            case { hasArguments: true, hasOptions: true }:
                LayoutEngine.WriteUsageClause(command.Path, GetArgumentToken(), GetOptionToken());
                break;
            
            case { hasArguments: true }:
                LayoutEngine.WriteUsageClause(command.Path, GetArgumentToken());
                break;
            
            case { hasOptions: true }:
                LayoutEngine.WriteUsageClause(command.Path, GetOptionToken());
                break;
            
            default:
                LayoutEngine.WriteUsageClause(command.Path);
                break;
        }

        return;

        UsageToken GetArgumentToken() => new(
            HelpElementKind.ArgumentUsageClauseToken,
            ResourceManager.GetUsageArgumentToken(command, symbols.ArgumentSymbols));

        UsageToken GetOptionToken() => new(
            HelpElementKind.OptionUsageClauseToken,
            ResourceManager.GetUsageOptionToken(command, symbols.OptionSymbols));
    }

    private void WriteOptionUsages(HelpModel helpModel, ICommand command)
    {
        var symbols = command is IRootCommand
            ? helpModel.Symbols.InterceptingSymbols
            : helpModel.Symbols
                .InterceptingSymbols
                .Where(symbol => symbol.Kind == AncillaryOptionKind.Help)
                .ToArray();

        if (symbols.Count == 0)
            return;

        foreach (var symbol in symbols)
        {
            LayoutEngine.WriteUsageClause(command.Path, new UsageToken(
                HelpElementKind.NamedOptionUsageClauseToken,
                ResourceManager.GetOptionAliasList(symbol)));
        }
    }
}