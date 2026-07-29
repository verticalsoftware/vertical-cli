using BasicDemo;
using Microsoft.Extensions.DependencyInjection;
using Vertical.Cli;
using Vertical.Cli.Configuration;
using Vertical.Cli.Validation;

var rootCommand = new RootCommand("app", Help.Root);
var compressCommand = new SubCommand("compress", Help.Compress);
compressCommand.SetHandler<IOptions, CompressHandler>();
rootCommand.AddSubCommand(compressCommand);

var app = new CommandLineApplication(rootCommand);
var services = new ServiceCollection();
services.AddSingleton<CompressHandler>();

app.ConfigureModel<IOptions>(builder => builder
    .MapVariadicArgument(x => x.SourceFiles, 0, Arity.OneOrMore, helpTopic: Help.SourceFiles)
    .MapOption(x => x.CompressionType, defaultProvider: () => CompressionAlgorithm.GZip, helpTopic: Help.CompressType)
    .MapOption(x => x.OutputFile, ["--out"], required: true, helpTopic: Help.OutputFile)
    .MapSwitch(x => x.PrintSha, ["--sha"], helpTopic: Help.PrintSha)
    .MapOption(x => x.SplitSizeKb, ["--split"], defaultProvider: () => 250,  helpTopic: Help.SplitSize,
        validate: ev => ev.MustBeLessOrEqualTo(500))
    .MapOption(x => x.Timeout, helpTopic: Help.Timeout));

app.HandleDirective("log",
    _ => Task.CompletedTask,
    DirectiveParameterArity.Required,
    helpTopic: Help.LogDirective);

app.HandleDirective("debug",
    _ => Task.CompletedTask,
    helpTopic: "Pauses the program so a debugger can be attached");

app.UseServices(services.BuildServiceProvider);
app.Configure();

return await app.RunAsync(args);
