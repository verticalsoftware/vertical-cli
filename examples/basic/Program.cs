using System.Globalization;
using BasicDemo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertical.Cli;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.Validation;

var rootCommand = new RootCommand("arcv");

var createCommand = new SubCommand("create");
createCommand.SetHandler<ICreateCommandOptions, CreateHandler>();
rootCommand.AddSubCommand(createCommand);

var app = new CommandLineApplication(rootCommand);

app.AddParameterizedDirective<LogLevel>(
    "log-level",
    ([GeneratedConversion] eventInfo) =>
    {
        eventInfo.ApplicationOptions.Configure<LoggingOptions>(options => options.LogLevel = eventInfo.Value);
        return Task.CompletedTask;
    });

app.ConfigureParser<ICompressionOptions>(builder => builder
    .MapOption(x => x.CompressionType, ["--alg"], defaultProvider: () => CompressionType.GZip));

app.ConfigureParser<IEncryptionOptions>(builder => builder
    .MapOption(x => x.EncryptionType, ["-e", "--encrypt"])
    .MapOption(x => x.Secret,
        validate: info =>
        {
            if (!info.Model.EncryptionType.HasValue || info.Value is { Length: > 0 })
                return;
            info.Error("secret required when encryption is active.");
        }));

app.ConfigureParser<IOverwriteOptions>(builder => builder.MapSwitch(x => x.Overwrite));

app.ConfigureParser<ICreateCommandOptions>(builder => builder
    .MapMultiValuedOption(
        x => x.ScanDirectories,
        ["--scan"],
        defaultProvider: () => [new DirectoryInfo(Directory.GetCurrentDirectory())],
        validate: collection => collection.EachValue(value => value.MustExist()))
    .MapOption(
        x => x.OutputDirectory,
        ["--out"],
        defaultProvider: () => new DirectoryInfo(Directory.GetCurrentDirectory()),
        validate: path => path.MustExist())
    .MapMultiValuedArgument(x => x.Patterns, ordinalPosition: 0, arity: Arity.OneOrMore)
    .MapOption(x => x.SplitSize,
        ["--split"],
        defaultProvider: () => SplitSize.Parse("250k", null))
    .MapSwitch(x => x.NoManifest)
    .MapMultiValuedOption<KeyValuePair<string, string>, Dictionary<string, string>>(
        x => x.Metadata,
        ["--md", "--metadata"])
);

app.AddArgumentConverter(KeyValuePairConverter.Instance);
app.AddCollectionConverter((IEnumerable<KeyValuePair<string, string>> values) => new Dictionary<string, string>(values));
    
var services = new ServiceCollection().AddSingleton<CreateHandler>();
app.UseServices(context =>
{
    var logLevel = context.ApplicationOptions.GetOptions<LoggingOptions>().LogLevel;
    return services
        .AddLogging(builder => builder
            .AddConsole(console =>
            {
                console.LogToStandardErrorThreshold = logLevel;
            })
            .SetMinimumLevel(logLevel))
        .BuildServiceProvider();
});

app.ConfigureHelp(options =>
{
    var path = Path.Combine("HelpResources", $"{CultureInfo.CurrentCulture.Name}.xml");
    options.HelpProvider = new XmlHelpProvider(() => File.OpenRead(path));
});

app.Configure();
return await app.RunAsync(args);
