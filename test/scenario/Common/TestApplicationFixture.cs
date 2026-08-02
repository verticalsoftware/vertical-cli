using Microsoft.Extensions.DependencyInjection;
using Microsoft.Testing.Platform.Logging;
using Vertical.Cli.Configuration;
using Vertical.Cli.IO;
using Vertical.Cli.Validation;

namespace Vertical.Cli.ScenarioTests.Common;

public sealed class TestApplicationFixture
{
    public TestApplicationFixture()
    {
        var services = new ServiceCollection();
        var console = new TestConsole();
        services.AddSingleton<IConsole>(console);
        services.AddSingleton<Handlers.CreateHandler>();
        services.AddSingleton<Handlers.ExtractHandler>();
        
        var rootCommand = new RootCommand("archive", HelpResources.Root);
        
        var createCommand = new SubCommand("create", HelpResources.CreateCommand);
        createCommand.SetHandler<ICreateCommandOptions, Handlers.CreateHandler>();
        rootCommand.AddSubCommand(createCommand);

        var extractCommand = new SubCommand("extract", HelpResources.ExtractCommand);
        extractCommand.SetHandler<IExtractCommandOptions, Handlers.ExtractHandler>();
        rootCommand.AddSubCommand(extractCommand);

        var app = new CommandLineApplication(rootCommand);
        
        app.ConfigureParser<ISharedOptions>(builder => builder
            .MapOption(x => x.CompressionType,
                ["-c", "--compression"],
                defaultProvider: () => CompressionType.GZip,
                helpTopic: HelpResources.CompressionTypeOption)
            .MapOption(x => x.EncryptionType,
                ["-e", "--encrypt"],
                defaultProvider: () => EncryptionType.RSA,
                helpTopic: HelpResources.EncryptionTypeOption)
            .MapOption(x => x.Timeout, 
                helpTopic: HelpResources.TimeoutOption,
                validate: info => info.MustBeLessThan(TimeSpan.FromMinutes(5)))
            .MapSwitch(x => x.ComputeSha, helpTopic: HelpResources.ComputeShaSwitch)
            .MapSwitch(x => x.Overwrite, helpTopic: HelpResources.OverwriteSwitch)
            .MapOption(x => x.SecretKey,
                ["--secret"],
                required: true,
                helpTopic: HelpResources.SecretOption)
        );

        app.ConfigureParser<ICreateOptions>(builder => builder
            .MapMultiValuedArgument<FileInfo, List<FileInfo>>(x => x.InputFiles,
                ordinalPosition: 0,
                arity: Arity.OneOrMore,
                helpTopic: HelpResources.InputFilesArgument)
            .MapOption(x => x.OutputFile,
                aliases: ["--out"],
                required: true,
                helpTopic: HelpResources.CompressOutputFileOption)
            .MapSwitch(x => x.IncludeMetadata, helpTopic: HelpResources.IncludeMetadataSwitch)
            .MapOption(x => x.OutputFileSplitSize,
                ["--split-size"],
                defaultProvider: () => new FileSize(250, "m"),
                helpTopic: HelpResources.OutputFileSplitSizeOption)
            .MapMultiValuedOption<KeyValuePair<string, string>, Dictionary<string, string>>(
                x => x.Properties,
                ["--property"],
                arity: Arity.ZeroOrMore,
                helpTopic: HelpResources.PropertiesOption));

        app.ConfigureParser<IExtractOptions>(builder => builder
            .MapArgument(
                x => x.InputFile,
                ordinalPosition: 0,
                required: true,
                helpTopic: HelpResources.InputFileArgument)
            .MapOption(
                x => x.OutputPath,
                ["--out"],
                defaultProvider: () => new DirectoryInfo(Directory.GetCurrentDirectory()),
                helpTopic: HelpResources.ExtractOutputPathOption)
        );

        // app.HandleUnboundSymbolWithParameter<LogLevel>(
        //     UnboundSymbolKind.Directive,
        //     ["log-level"],
        //     _ => Task.CompletedTask,
        //     helpTopic: HelpResources.LogLevelDirective);

        app.AddParameterizedDirective<LogLevel>(
            "log-level",
            _ => Task.CompletedTask,
            helpTopic: HelpResources.LogLevelDirective);

        app.AddArgumentConverter(KeyValuePairConverter.Convert);
        app.AddCollectionConverter((IEnumerable<KeyValuePair<string, string>> values) =>
            new Dictionary<string, string>(values));
        app.UseServices(_ => services.BuildServiceProvider());
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

    public TestConsole Console { get; }

    public CommandLineApplication Application { get; }
}