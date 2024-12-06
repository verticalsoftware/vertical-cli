using Vertical.Cli.Configuration;
using Vertical.Cli.Routing;

namespace Vertical.Cli.Help;

internal abstract class AbstractFormatProvider(
    HelpFormattingOptions formattingOptions,
    Func<TextWriter> textWriterFactory,
    int renderWidth) : IHelpProvider
{
    public record DisplayParameter(
        string IdentifierSyntax,
        string OperandSyntax,
        string Description);
    
    /// <inheritdoc />
    public async Task WriteContentAsync(HelpContext context)
    {
        await using var writer = textWriterFactory();
        WriteInternalAsync(writer, context);
        await writer.FlushAsync();
    }

    private void WriteInternalAsync(TextWriter textWriter, HelpContext context)
    {
        var layoutWriter = new HelpLayoutWriter(textWriter, formattingOptions, renderWidth);

        WriteRouteTitleSection(layoutWriter, context);
        WriteUsageSection(layoutWriter, context);
        WriteSubRouteSection(layoutWriter, context);
        WriteParameters(layoutWriter, context);
    }

    private void WriteSubRouteSection(HelpLayoutWriter layoutWriter, HelpContext context)
    {
        var rootPath = context.Route.Path;
        var routeGroups = formattingOptions
            .RouteGroupsProvider(context.ChildRoutes)
            .SelectMany(group => group.Select(route => new
            {
                key = group.Key,
                route,
                path = rootPath.GetDescendantPath(route.Path)
            }))
            .GroupBy(i => i.key)
            .ToArray();

        if (routeGroups.Length == 0)
            return;

        var padding = routeGroups
            .SelectMany(group => group)
            .Max(item => item.path.Length) + 5 + formattingOptions.IndentSpaces;

        foreach (var group in routeGroups)
        {
            layoutWriter.WriteLine(HelpElement.SectionTitle, group.Key);

            foreach (var item in group)
            {
                layoutWriter
                    .Indent()
                    .Write(HelpElement.UsageIdentifier, item.path)
                    .IndentTo(padding)
                    .WriteAnchored(HelpElement.Description, formattingOptions.RouteDescriptionFormatter(item.route))
                    .WriteLine();
            }

            layoutWriter.EnqueueBreak();
        }
    }

    private void WriteParameters(HelpLayoutWriter layoutWriter, HelpContext context)
    {
        var optionGroups = formattingOptions
            .OptionsGroupsProvider(context.Options)
            .SelectMany(group => group.Select(option => new
            {
                key = group.Key,
                option,
                identifierSyntax = formattingOptions.IdentifierListFormatter(option),
                operandSyntax = formattingOptions.OperandFormatter(option),
                description = formattingOptions.ParameterDescriptionFormatter(option),
            }))
            .GroupBy(i => i.key, i => new DisplayParameter(i.identifierSyntax, i.operandSyntax, i.description))
            .ToArray();

        var argumentGroups = formattingOptions
            .ArgumentGroupsProvider(context.Arguments)
            .SelectMany(group => group.Select(argument => new
            {
                key = group.Key,
                argument,
                identifierSyntax = formattingOptions.IdentifierListFormatter(argument),
                description = formattingOptions.ParameterDescriptionFormatter(argument)
            }))
            .GroupBy(i => i.key, i => new DisplayParameter(i.identifierSyntax, string.Empty, i.description))
            .ToArray();

        if (optionGroups.Length == 0 && argumentGroups.Length == 0)
            return;

        var optionPadding = optionGroups.Length > 0
            ? optionGroups.SelectMany(group => group).Max(opt => opt.IdentifierSyntax.Length + opt.OperandSyntax.Length)
            : 0;
        var argumentPadding = argumentGroups.Length > 0
            ? argumentGroups.SelectMany(group => group).Max(arg => arg.IdentifierSyntax.Length)
            : 0;
        var padding = Math.Max(optionPadding, argumentPadding) + 8;

        WriteParameterGroups(layoutWriter, argumentGroups, padding);
        WriteParameterGroups(layoutWriter, optionGroups, padding);
    }

    private void WriteParameterGroups(HelpLayoutWriter layoutWriter,
        IGrouping<string, DisplayParameter>[] groups, 
        int padding)
    {
        if (groups.Length == 0)
            return;
        
        foreach (var group in groups)
        {
            WriteParameterGroup(layoutWriter, group.Key, group, padding);
        }
    }

    protected abstract void WriteParameterGroup(HelpLayoutWriter layoutWriter,
        string key,
        IEnumerable<DisplayParameter> group,
        int padding);

    private void WriteRouteTitleSection(HelpLayoutWriter layoutWriter, HelpContext context)
    {
        var content = formattingOptions.RouteDescriptionFormatter(context.Route);
        if (string.IsNullOrWhiteSpace(content))
            return;
        
        layoutWriter
            .Write(HelpElement.SectionTitle, formattingOptions.SectionTitleFormatter(HelpSection.Description))
            .WriteLine()
            .Indent()
            .WriteAnchored(HelpElement.RouteDescription, formattingOptions.RouteDescriptionFormatter(context.Route))
            .WriteLine()
            .EnqueueBreak();
    }

    private void WriteUsageSection(HelpLayoutWriter layoutWriter, HelpContext context)
    {
        layoutWriter
            .Write(HelpElement.SectionTitle, formattingOptions.SectionTitleFormatter(HelpSection.Usage))
            .WriteLine();

        TryWriteCallableUsage(layoutWriter, context);
        TryWriteChildRouteUsage(layoutWriter, context);
        WriteHelpUsage(layoutWriter, context);

        layoutWriter.EnqueueBreak();
    }

    private void TryWriteCallableUsage(HelpLayoutWriter layoutWriter, HelpContext context)
    {
        if (!context.Route.IsCallable)
            return;
        
        layoutWriter
            .Indent()
            .Write(HelpElement.UsageIdentifier, context.Route.Path.ToString())
            .EnqueueSpace();

        TryWriteArgumentsToken(layoutWriter, context.Route, context.Arguments);
        TryWriteOptionsToken(layoutWriter, context.Route, context.Options);

        layoutWriter.WriteLine();
    }

    private void TryWriteArgumentsToken(HelpLayoutWriter layoutWriter, RouteDefinition route, CliParameter[] arguments)
    {
        if (arguments.Length == 0)
            return;

        layoutWriter
            .Write(HelpElement.UsageArgumentList, formattingOptions.ArgumentsUsageFormatter(route, arguments))
            .EnqueueSpace();
    }

    private void TryWriteOptionsToken(HelpLayoutWriter layoutWriter, RouteDefinition route, CliParameter[] options)
    {
        if (options.Length == 0)
            return;

        layoutWriter
            .Write(HelpElement.UsageOptionList, formattingOptions.OptionsUsageFormatter(route, options))
            .EnqueueSpace();
    }

    private void TryWriteChildRouteUsage(HelpLayoutWriter layoutWriter, HelpContext context)
    {
        if (context.ChildRoutes.Length == 0)
            return;

        layoutWriter
            .Indent()
            .Write(HelpElement.UsageIdentifier, context.Route.Path.ToString())
            .EnqueueSpace()
            .WriteLine(HelpElement.UsageArgumentList, formattingOptions.SubRouteUsageFormatter(context.Route));
    }

    private static void WriteHelpUsage(HelpLayoutWriter layoutWriter, HelpContext context)
    {
        var optionalCommand = context.ChildRoutes.Length > 0
            ? "[Command] "
            : string.Empty;
        
        layoutWriter
            .Indent()
            .Write(HelpElement.UsageIdentifier, context.Route.Path.ToString())
            .EnqueueSpace()
            .WriteLine(HelpElement.UsageArgumentList, $"{optionalCommand}{context.HelpIdentifier}");
    }
}