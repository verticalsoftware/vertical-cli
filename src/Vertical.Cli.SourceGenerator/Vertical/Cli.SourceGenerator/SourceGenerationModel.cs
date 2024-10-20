namespace Vertical.Cli.SourceGenerator;

public sealed class SourceGenerationModel
{
    public SourceGenerationModel(IReadOnlyCollection<CommandDefinition> commandDefinitions)
    {
        RootDefinition = commandDefinitions.First(def => def.IsRootDefinition);
        SubDefinitions = commandDefinitions.Where(def => !def.IsRootDefinition).ToArray();
    }

    public CommandDefinition[] SubDefinitions { get; set; }

    public CommandDefinition RootDefinition { get; set; }
}