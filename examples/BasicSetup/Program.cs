using Vertical.Cli;
using Vertical.Cli.Parsing;

// Defines the root command (aka the app)
var root = new RootCommand<Options>("arc", "An archiving utility.");
root
    // Only applies to sub-commands
    .AddOption(x => x.Compression,
        names: ["--compression"],
        scope: CliScope.Descendants,
        defaultProvider: () => Compression.Zip,
        description: "Compression type to use (None, AutoDetect, Zip, Deflate)",
        operandSyntax: "None|GZip|Deflate")
    ;
    
// Add the create mode command
root
    .AddSubCommand<CreateOptions>("create", "Creates a new archive.")
    .AddOption(x => x.OutputFile,
        names: ["-o", "--out"],
        description: "Path/name of the output file.",
        arity: Arity.One, // require value
        validation: value => value.DoesNotExist()) // don't overwrite existing file
    .AddSwitch(x => x.ComputeChecksum,
        names: ["--check", "--compute-checksum"],
        description: "Compute and display the checksum on the output file.")
    .AddArgument(x => x.InputFiles,
        Arity.OneOrMany,
        description: "One or more input files or glob patterns to add to the archive")
    .HandleAsync(async (options, cancellationToken) =>
    {
        // Create the archive...
        await Task.CompletedTask;
    });

// Add the extract mode command
root
    .AddSubCommand<ExtractOptions>("extract", "Extracts an archive.")
    .AddOption(x => x.Checksum,
        names: ["--check"],
        description: "Validates the archive's checksum")
    .AddArgument(x => x.InputFile,
        arity: Arity.One,
        description: "Path to the input file",
        validation: value => value.Exists())
    .AddOption(x => x.OutputDirectory,
        names: ["-o", "--out"],
        defaultProvider: () => new DirectoryInfo(Directory.GetCurrentDirectory()),
        description: "Path to the output directory")
    .HandleAsync(async (options, cancellationToken) =>
    {
        // Extract the archive
        await Task.CompletedTask;
    });

// Adds a switch [--help, ?, -?] that triggers the help system
root.AddHelpSwitch();

root.ConfigureOptions(options =>
{
    options.ArgumentPreProcessors = 
    [
        (args, next) =>
        {
            EnvironmentVariablePreProcessor.Handle(args);
            ResponseFilePreProcessor.Handle(args);
            EnvironmentVariablePreProcessor.Handle(args, next);
        }
    ];
});

try
{
    return await root.InvokeAsync(args);
}
catch (CommandLineException exception)
{
    Console.WriteLine(exception.Message);
    return -1;
}

// Models
enum Compression
{
    None,
    AutoDetect,
    Zip,
    Deflate
}

abstract class Options
{
    public Compression Compression { get; set; }
}

class CreateOptions : Options
{
    public bool ComputeChecksum { get; set; }
    public FileInfo OutputFile { get; set; } = default!;
    public IEnumerable<FileInfo> InputFiles { get; set; } = default!;
}

class ExtractOptions : Options
{
    public string? Checksum { get; set; }
    public FileInfo InputFile { get; set; } = default!;
    public DirectoryInfo OutputDirectory { get; set; } = default!;
}