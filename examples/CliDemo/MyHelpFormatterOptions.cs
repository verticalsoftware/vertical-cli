using Vertical.Cli.Help;

namespace CliDemo;

public static class MyHelpFormatterOptions
{
    public static HelpFormattingOptions Instance = new()
    {
        // Instead of embedding help content in the code, for this demo we'll pull the content
        // from a resource
        RouteDescriptionFormatter = route => route.GetHelpTopic(),
        ParameterDescriptionFormatter = parameter => parameter.GetHelpTopic(),
        
        // Categorizes the options
        OptionsGroupsProvider = options => options.GroupBy(opt => opt switch
            {
                { ModelType.Name: nameof(CommonOptions) } => "Common options:",
                _ => "Operation options:"
            })
            .OrderByDescending(group => group.Key),
        
        // Render the operands in a different color
        OutputFormatter = (element, str) => element == HelpElement.OperandSyntax
            ? $"\x1b[38;5;175m{str}\x1b[0m" 
            : HelpFormattingOptions.Default.OutputFormatter(element, str)
    };
}