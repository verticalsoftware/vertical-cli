using System.Globalization;
using BasicDemo;
using Microsoft.Extensions.DependencyInjection;
using Vertical.Cli;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.Validation;

var rootCommand = new RootCommand("app", Help.Root);
var compressCommand = new SubCommand("compress", Help.Compress);
compressCommand.SetHandler<IOptions, CompressHandler>();
rootCommand.AddSubCommand(compressCommand);

var app = new CommandLineApplication(rootCommand);
var services = new ServiceCollection();
services.AddSingleton<CompressHandler>();

app.ConfigureParser<IOptions>(builder => builder
    .MapMultiValuedArgument(x => x.SourceFiles, 
        0, 
        Arity.OneOrMore, 
        validate: collectionContext => collectionContext
            .EachValue(valueContext => valueContext.MustExist()),
        helpTopic: Help.SourceFiles)
    .MapOption(x => x.CompressionType, defaultProvider: () => CompressionAlgorithm.GZip, helpTopic: Help.CompressType)
    .MapOption(x => x.OutputFile, ["--out"], required: true, helpTopic: Help.OutputFile)
    .MapSwitch(x => x.PrintSha, ["--sha"], helpTopic: Help.PrintSha)
    .MapOption(x => x.SplitSizeKb, ["--split"], defaultProvider: () => 250,  helpTopic: Help.SplitSize,
        validate: ev => ev.MustBeLessOrEqualTo(500))
    .MapOption(x => x.Timeout, helpTopic: Help.Timeout));

app.AddParameterizedDirective<LogSeverity>(
    "log-level",
    info =>
    {
        Console.WriteLine($"Setting log level to {info.Value}");
        info.ApplicationOptions.Configure<AppOptions>(options => options.LogSeverity = info.Value);
        return Task.CompletedTask;
    },
    helpTopic: Help.LogDirective);

app.AddArgumentConverter(Converters.Enum<LogSeverity>());

app.ConfigureHelp(options => options.HelpProvider = new XmlHelpProvider(() => 
    File.OpenRead($"HelpResources/{CultureInfo.CurrentCulture.Name}.xml")));
app.UseServices(_ => services.BuildServiceProvider());
app.Configure();

return await app.RunAsync(args);
