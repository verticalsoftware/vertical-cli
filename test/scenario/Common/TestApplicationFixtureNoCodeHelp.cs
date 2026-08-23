using Microsoft.Testing.Platform.Logging;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Validation;

namespace Vertical.Cli.ScenarioTests.Common;

public sealed class TestApplicationFixtureNoCodeHelp
{
    public TestApplicationFixtureNoCodeHelp()
    {
        var console = new TestConsole();
        
        var rootCommand = new RootCommand("archive");
        
        var createCommand = new SubCommand("create");
        createCommand.SetHandler(context => new Handlers.CreateHandler(context.Configuration.Console));
        rootCommand.AddSubCommand(createCommand);

        var extractCommand = new SubCommand("extract");
        extractCommand.SetHandler(context => new Handlers.ExtractHandler(context.Configuration.Console));
        rootCommand.AddSubCommand(extractCommand);

        var app = new CommandLineApplication(rootCommand);

        app.ConfigureMiddleware(middleware => middleware
            .AddSwitch(
                "Version",
                "--version",
                _ => Task.FromResult<int?>(0),
                "Display version information."));
        
        app.ConfigureParser<ISharedOptions>(builder => builder
            .ParseOption(x => x.CompressionType,
                ["-c", "--compression"],
                useDefault: () => CompressionType.GZip)
            .ParseOption(x => x.EncryptionType,
                ["-e", "--encrypt"],
                useDefault: () => EncryptionType.RSA)
            .ParseOption(x => x.Timeout, 
                validate: info => info.MustBeLessThan(TimeSpan.FromMinutes(5)))
            .ParseSwitch(x => x.ComputeSha)
            .ParseSwitch(x => x.Overwrite)
            .ParseOption(x => x.SecretKey,
                ["--secret"],
                required: true)
        );

        app.ConfigureParser<ICreateOptions>(builder => builder
            .ParseRepeatableArgument<FileInfo, List<FileInfo>>(x => x.InputFiles,
                ordinalPosition: 0,
                arity: Arity.OneOrMore)
            .ParseOption(x => x.OutputFile,
                aliases: ["--out"],
                required: true)
            .ParseSwitch(x => x.IncludeMetadata)
            .ParseOption(x => x.OutputFileSplitSize,
                ["--split-size"],
                useDefault: () => new FileSize(250, "m"))
            .ParseRepeatableOption<KeyValuePair<string, string>, Dictionary<string, string>>(
                x => x.Properties,
                ["--property"],
                arity: Arity.ZeroOrMore));

        app.ConfigureParser<IExtractOptions>(builder => builder
            .ParseArgument(
                x => x.InputFile,
                ordinalPosition: 0,
                required: true)
            .ParseOption(
                x => x.OutputPath,
                ["--out"],
                useDefault: () => new DirectoryInfo(Directory.GetCurrentDirectory()))
        );

        app.ConfigureMiddleware(middleware => middleware
            .AddDirective<LogLevel>(
                "log-level",
                _ => Task.CompletedTask));

        app.AddArgumentConverter(KeyValuePairConverter.Convert);
        app.AddArgumentConverter(Converters.Enum<LogLevel>());
        app.AddCollectionConverter((IEnumerable<KeyValuePair<string, string>> values) =>
            new Dictionary<string, string>(values));
        app.UseConsole(console);

        Application = app;
        Console = console;
    }

    public async Task<string> GetOutputAsync(string[] args)
    {
        var app = Application;
        
        app.Configure();
        _ = await app.RunAsync(args);
        return Console.ToString();
    }

    public static async Task<string> GetOutputAsync(CommandLineApplication app, string[] args)
    {
        var console = new TestConsole();
        app.UseConsole(console);
        _ = await app.RunAsync(args);
        return console.ToString();
    }

    public TestConsole Console { get; }

    public CommandLineApplication Application { get; }
}