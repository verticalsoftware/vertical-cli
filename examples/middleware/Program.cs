using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertical.Cli;
using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.DependencyInjection;
using Vertical.Cli.Invocation;

var command = new RootCommand("app", "Says hello back to you.");
command.SetHandlerService<IOptions, Handler>();

var app = new CommandLineApplication(command);
app.ConfigureParser<IOptions>(parser => parser
    .ParseArgument(
        x => x.Name,
        ordinalPosition: 0,
        required: true,
        helpTopic: "Name of the current user."));

app.ConfigureMiddleware(middleware =>
{
    // Add a directive that lets the user control the log level
    middleware.AddDirective<LogLevel>(
        "log-level",
        ([GeneratedConversion] eventInfo) =>
        {
            eventInfo.Context.AppData.SetValue(eventInfo.Value);
            return Task.CompletedTask;
        },
        helpTopic: "Set the severity level of the logger.");
    
    // Add a version option
    middleware.AddSwitch(
        "Version",
        "--version",
        context =>
        {
            context.OutputWriter.WriteLine("Middleware demo v1.0");
            return Task.CompletedTask;
        },
        helpTopic: "Display the application's version.");
    
    // Add a middleware that lets the user attach the debugger
    middleware.AddLast(async (context, next) =>
    {
        context.OutputWriter.Write("Attach debugger then press any key...");
        _ = Console.ReadKey(intercept: true);
        context.OutputWriter.WriteLine();
        await next(context);
    });
});

app.ConfigureServices((context, services) =>
{
    services.AddSingleton<Handler>();
    
    var logLevel = context.AppData.GetValueOrDefault(LogLevel.Information);
    services.AddLogging(builder => builder
        .SetMinimumLevel(logLevel)
        .AddConsole());
});

app.Configure();
return await app.RunAsync(args);

// Model definition
[GeneratedBinding]
internal interface IOptions
{
    string Name { get; }
}

// Command handler
internal sealed class Handler(ILogger<Handler> logger) : IHandler<IOptions>
{
    /// <inheritdoc />
    public Task<int> HandleAsync(IOptions options, CancellationToken cancellationToken)
    {
        logger.LogDebug("Verbose logging enabled");
        logger.LogInformation("Hello {name}!", options.Name);
        return Task.FromResult(0);
    }
}