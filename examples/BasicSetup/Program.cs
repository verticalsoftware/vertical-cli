using Vertical.Cli;
using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Validation;

var rootCommand = new RootCommand<BaseModel>("dotnet", description: ".NET SDK tooling");

rootCommand
    .AddHelpSwitch()
    .ConfigureOptions(options => options.HelpProvider = new DefaultHelpProvider(new DefaultHelpOptions
    {
        DoubleSpace = true,
        NameComparer = IdentifierComparer.Sorted
    }))
    .AddAction(["-v", "--version"], () => Console.WriteLine("1.0.0-dev"), description: "Displays the application version")
    .AddOption(x => x.Verbosity, ["--verbosity"],
        scope: CliScope.Descendants,
        description: "Output verbosity (Verbose, Normal, Minimal)");

rootCommand.AddSubCommand<BuildModel>(["build"], "Builds a .NET assembly")
    .AddArgument(x => x.ProjectPath, Arity.One,
        description: "Path to the project file",
        validation: v => v.Exists())
    .AddOption(x => x.Configuration, ["-c", "--configuration"],
        defaultProvider: () => "Debug",
        description: "Configuration name to build")
    .AddOption(x => x.OutputDirectory, ["-o", "--output"],
        defaultProvider: () => new DirectoryInfo(Directory.GetCurrentDirectory()),
        description: "Directory where the built artifacts will be written")
    .AddSwitch(x => x.Force, ["-f", "--force"],
        description: "Forces all dependencies to be resolved")
    .AddSwitch(x => x.NoRestore, ["--no-restore"],
        description: "Don't execute implicit restore")
    .HandleAsync(async (model, cancellation) =>
    {
        Console.WriteLine($"Building {model.ProjectPath}, configuration={model.Configuration}");
        await Task.CompletedTask;
    });

try
{
    await rootCommand.InvokeAsync(args, CancellationToken.None);
}
catch (CommandLineException exception)
{
    Console.WriteLine(exception.Message);
}

public enum Verbosity { Verbose, Normal, Minimal }

public abstract class BaseModel
{
    public Verbosity Verbosity { get; set; }
}

public class BuildModel : BaseModel
{
    public FileInfo ProjectPath { get; set; } = default!;
    public string Configuration { get; set; } = default!;
    public DirectoryInfo OutputDirectory { get; set; } = default!;
    public bool Force { get; set; }
    public bool NoRestore { get; set; }
}