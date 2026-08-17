using System.Globalization;
using BasicDemo;
using Microsoft.Extensions.Logging;
using Vertical.Cli;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.Validation;

var rootCommand = new RootCommand("arcv");

var createCommand = new SubCommand("create");
createCommand.SetHandler<ICreateCommandOptions>((_, _) => Task.FromResult(0));
rootCommand.AddSubCommand(createCommand);

var app = new CommandLineApplication(rootCommand);

app.ConfigureMiddleware(middleware => middleware
    .AddSwitch(
        "Version",
        "--version",
        _ =>
        {
            Console.WriteLine("BasicDemo v1.0");
            return Task.FromResult<int?>(0);
        })
    .AddDirective<LogLevel>(
        "log-level",
        ([GeneratedConversion] eventInfo) =>
        {
            eventInfo.Context.AppData.Configure<LoggingOptions>(options =>
                options.LogLevel = eventInfo.Value);
            return Task.CompletedTask;
        }));

app.ConfigureParser<ICompressionOptions>(builder => builder
    .ParseOption(x => x.CompressionType, ["--alg"], useDefault: () => CompressionType.GZip));

app.ConfigureParser<IEncryptionOptions>(builder => builder
    .ParseOption(x => x.EncryptionType, ["-e", "--encrypt"])
    .ParseOption(x => x.Secret,
        validate: info =>
        {
            if (!info.Model.EncryptionType.HasValue || info.Value is { Length: > 0 })
                return;
            info.Error("secret required when encryption is active.");
        }));

app.ConfigureParser<IOverwriteOptions>(builder => builder.ParseSwitch(x => x.Overwrite));

app.ConfigureParser<ICreateCommandOptions>(builder => builder
    .ParseRepeatableOption(
        x => x.ScanDirectories,
        ["--scan"],
        useDefault: () => [new DirectoryInfo(Directory.GetCurrentDirectory())],
        validate: collection => collection.EachValue(value => value.MustExist()))
    .ParseOption(
        x => x.OutputDirectory,
        ["--out"],
        useDefault: () => new DirectoryInfo(Directory.GetCurrentDirectory()),
        validate: path => path.MustExist())
    .ParseRepeatableArgument(x => x.Patterns, ordinalPosition: 0, arity: Arity.OneOrMore)
    .ParseOption(
        x => x.SplitSize,
        ["--split"],
        useDefault: () => SplitSize.Parse("250k", null))
    .ParseSwitch(x => x.NoManifest)
    .ParseRepeatableOption<KeyValuePair<string, string>, Dictionary<string, string>>(
        x => x.Metadata,
        ["--md", "--metadata"])
);

app.AddArgumentConverter(KeyValuePairConverter.Instance);
app.AddCollectionConverter((IEnumerable<KeyValuePair<string, string>> values) => new Dictionary<string, string>(values));
    
app.ConfigureHelp(options =>
{
    var path = Path.Combine("HelpResources", $"{CultureInfo.CurrentCulture.Name}.xml");
    options.HelpProvider = new XmlHelpProvider(() => File.OpenRead(path));
});

app.Configure();
return await app.RunAsync(args);
