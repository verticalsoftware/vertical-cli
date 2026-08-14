using Microsoft.Testing.Platform.Logging;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Validation;

namespace Vertical.Cli.ScenarioTests.Common;

public sealed class TestApplicationFixture
{
    public TestApplicationFixture()
    {
        var console = new TestConsole();
        
        var rootCommand = new RootCommand("archive", HelpResources.Root);
        rootCommand.AddUnboundOption(
            "Version",
            ["--version"],
            UnboundScope.Global,
            (_,_) => Task.CompletedTask,
            "Displays version information.");
        
        var createCommand = new SubCommand("create", HelpResources.CreateCommand);
        createCommand.SetHandler(context => new Handlers.CreateHandler(context.Configuration.Console));
        rootCommand.AddSubCommand(createCommand);

        var extractCommand = new SubCommand("extract", HelpResources.ExtractCommand);
        extractCommand.SetHandler(context => new Handlers.ExtractHandler(context.Configuration.Console));
        rootCommand.AddSubCommand(extractCommand);

        var app = new CommandLineApplication(rootCommand);
        
        app.ConfigureParser<ISharedOptions>(builder => builder
            .ParseOption(x => x.CompressionType,
                ["-c", "--compression"],
                defaultProvider: () => CompressionType.GZip,
                helpTopic: HelpResources.CompressionTypeOption)
            .ParseOption(x => x.EncryptionType,
                ["-e", "--encrypt"],
                defaultProvider: () => EncryptionType.RSA,
                helpTopic: HelpResources.EncryptionTypeOption)
            .ParseOption(x => x.Timeout, 
                helpTopic: HelpResources.TimeoutOption,
                validate: info => info.MustBeLessThan(TimeSpan.FromMinutes(5)))
            .ParseSwitch(x => x.ComputeSha, helpTopic: HelpResources.ComputeShaSwitch)
            .ParseSwitch(x => x.Overwrite, helpTopic: HelpResources.OverwriteSwitch)
            .ParseOption(x => x.SecretKey,
                ["--secret"],
                required: true,
                helpTopic: HelpResources.SecretOption)
        );

        app.ConfigureParser<ICreateOptions>(builder => builder
            .ParseRepeatableArgument<FileInfo, List<FileInfo>>(x => x.InputFiles,
                ordinalPosition: 0,
                arity: Arity.OneOrMore,
                helpTopic: HelpResources.InputFilesArgument)
            .ParseOption(x => x.OutputFile,
                aliases: ["--out"],
                required: true,
                helpTopic: HelpResources.CompressOutputFileOption)
            .ParseSwitch(x => x.IncludeMetadata, helpTopic: HelpResources.IncludeMetadataSwitch)
            .ParseOption(x => x.OutputFileSplitSize,
                ["--split-size"],
                defaultProvider: () => new FileSize(250, "m"),
                helpTopic: HelpResources.OutputFileSplitSizeOption)
            .ParseRepeatableOption<KeyValuePair<string, string>, Dictionary<string, string>>(
                x => x.Properties,
                ["--property"],
                arity: Arity.ZeroOrMore,
                helpTopic: HelpResources.PropertiesOption));

        app.ConfigureParser<IExtractOptions>(builder => builder
            .ParseArgument(
                x => x.InputFile,
                ordinalPosition: 0,
                required: true,
                helpTopic: HelpResources.InputFileArgument)
            .ParseOption(
                x => x.OutputPath,
                ["--out"],
                defaultProvider: () => new DirectoryInfo(Directory.GetCurrentDirectory()),
                helpTopic: HelpResources.ExtractOutputPathOption)
        );

        app.HandleParameterizedDirective<LogLevel>(
            "log-level",
            _ => Task.CompletedTask,
            helpTopic: HelpResources.LogLevelDirective);

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