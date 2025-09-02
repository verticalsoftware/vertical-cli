using System.Text;
using System.Text.RegularExpressions;
using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.IO;

namespace Vertical.Cli.Invocation;

public partial class EndToEndTests
{
    public enum CompressionType
    {
        GZip,
        Deflate
    }

    public record ArchiveOptions(
        CompressionType Compression,
        FileInfo SourcePath,
        FileInfo DestPath,
        TimeSpan? Timeout,
        bool Encrypt);

    private readonly IConsole _console = new StringConsole();
    private readonly CommandLineApplication _unit;

    public EndToEndTests() => _unit = BuildApplication(_console);

    [Fact]
    public async Task Help_Displayed()
    {
        await _unit.RunAsync(["--help"], _console);
        await Verify(_console.ToString());
    }
    
    [Fact]
    public async Task Create_Help_Displayed()
    {
        await _unit.RunAsync(["create", "--help"], _console);
        await Verify(_console.ToString());
    }
    
    [Fact]
    public async Task Extract_Help_Displayed()
    {
        await _unit.RunAsync(["extract", "--help"], _console);
        await Verify(_console.ToString());
    }

    [Fact]
    public async Task Arity_Error_Reported()
    {
        await _unit.RunAsync(["create", "--compression:gzip", "./var/temp/source.txt"], _console);
        await Verify(_console.ToString());
    }

    [Fact]
    public async Task Conversion_Error_Reported()
    {
        await _unit.RunAsync(["create", "--timeout:30", "./var/source.txt", "./var/dest.txt"], _console);
        await Verify(_console.ToString());
    }
    
    [Fact]
    public async Task Parameter_Error_Reported()
    {
        await _unit.RunAsync(["create", "./var/source.txt", "./var/dest.txt", "--timeout"], _console);
        await Verify(_console.ToString());
    }
    
    [Fact]
    public async Task Switch_Error_Reported()
    {
        await _unit.RunAsync(["create", "./var/source.txt", "./var/dest.txt", "--encrypt:yes"], _console);
        await Verify(_console.ToString());
    }

    [Fact]
    public async Task Invalid_Symbol_Reported()
    {
        await _unit.RunAsync(["create", "./var/source.txt", "./var/dest.txt", "--test"], _console);
        await Verify(_console.ToString());
    }
    
    [Fact]
    public async Task Extraneous_Argument_Reported()
    {
        await _unit.RunAsync(["create", "./var/source.txt", "./var/dest.txt", "./var/dest.txt"], _console);
        await Verify(_console.ToString());
    }

    [Fact]
    public async Task Directive_Detected()
    {
        await _unit.RunAsync(["[log-level:info]", "create", "./var/source.txt", "./var/dest.txt"], _console);
        await Verify(_console.ToString());
    }

    private static CommandLineApplication BuildApplication(IConsole console)
    {
        var rootCommand = new RootCommand("fs", helpTag: "Create or extract archive files");
        
        rootCommand.AddCommand(new Command<ArchiveOptions>(
            "create",
            (options, _) =>
            {
                console.WriteLine($"Creating archive {options.SourcePath} -> {options.DestPath} (using {options.Compression})");
                return Task.FromResult(1);
            },
            "Create a new archive file compressed using either the GZip or Deflate algorithm"));
        
        rootCommand.AddCommand(new Command<ArchiveOptions>(
            "extract",
            (options, _) =>
            {
                console.WriteLine($"Creating archive {options.SourcePath} -> {options.DestPath} (using {options.Compression})");
                return Task.FromResult(1);
            },
            "Extract an existing archive file"));

        var builder = new CommandLineBuilder(rootCommand)
            .ConfigureModel<ArchiveOptions>(model => model
                .AddOption(x => x.Compression,
                    ["-c", "--compression"],
                    helpTag: "The compression algorithm to use, one of:\n  - GZip (default)\n  - Deflate",
                    setBindingOptions: binder => binder.SetDefaultValue(CompressionType.GZip))
                .AddArgument(x => x.SourcePath,
                    0,
                    "SOURCE",
                    helpTag: "The source file to compress or extract",
                    configureValidation: args =>
                    {
                        if (args.Value.Name == "source.txt")
                            return;
                        args.AddValidationError("Invalid file path");
                    })
                .AddArgument(x => x.DestPath,
                    1,
                    "DEST",
                    helpTag: "The destination file to write",
                    configureValidation: args =>
                    {
                        if (args.Value.Name == "dest.txt")
                            return;
                        args.AddValidationError("Invalid file path");
                    })
                .AddOption(x => x.Timeout,
                    ["--timeout"],
                    helpTag: new SymbolHelpTag(
                        "A timeout value that cancels the operation if the interval is reached.",
                        "<VALUE[h|m|s]>"))
                .AddSwitch(x => x.Encrypt,
                    helpTag: "Indicates an encryption operation should be performed on the source file")
                .UseBinder(context => new ArchiveOptions(
                    context.GetValue(x => x.Compression, Converters.Enum<CompressionType>()),
                    context.GetValue(x => x.SourcePath, Converters.FileInfo),
                    context.GetValue(x => x.DestPath, Converters.FileInfo),
                    context.GetValue(x => x.Timeout, null),
                    context.GetValue(x => x.Encrypt, Converters.Boolean))
                ));

        builder.AddValueConverter<TimeSpan?>(str =>
        {
            if (MyRegex().Match(str) is { Success: true } match)
            {
                var value = int.Parse(match.Groups[1].Value);
                return match.Groups[2].Value switch
                {
                    "h" => TimeSpan.FromHours(value),
                    "m" => TimeSpan.FromMinutes(value),
                    _ => TimeSpan.FromSeconds(value)
                };
            }

            throw new FormatException("invalid timeout value");
        });

        builder.ConfigureMiddleware(middleware => middleware
            .AddDirectiveHandler(args =>
            {
                if (args.Token.Text != "[log-level:info]")
                    return Task.CompletedTask;
                
                args.DequeueToken();
                console.WriteLine("Set log-level = info");
                return Task.CompletedTask;
            })
            .HandleErrors());

        builder.Options.HelpProviderFactory = @out => new DefaultHelpProvider(
            new CompactLayoutEngine(
                new NonDecoratedHelpWriter(@out.Out), 120),
            new HelpTagResourceManager());

        return builder.Build();
    }

    [GeneratedRegex("([0-9]+)(h|m|s)")]
    private static partial Regex MyRegex();
}